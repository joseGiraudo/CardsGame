using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs;
using DataAccessLibrary.DAOs.Interface;
using ModelsLibrary.DTOs.Tournament;
using ModelsLibrary.Enums;
using ModelsLibrary.Models;
using Org.BouncyCastle.Cms;
using ServicesLibrary.Exceptions;
using ServicesLibrary.Services.Interface;

namespace ServicesLibrary.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly ITournamentDAO _tournamentDAO;
        private readonly ITournamentPlayerDAO _tournamentPlayerDAO;
        private readonly IGameDAO _gameDAO;
        private readonly IUserDAO _userDAO;
        private readonly IPlayerCardDAO _playerCardDAO;
        private readonly ISeriesDAO _seriesDAO;

        public TournamentService(ITournamentDAO tournamentDAO, ITournamentPlayerDAO tournamentPlayerDAO,
            IGameDAO gameDAO, IUserDAO userDAO, IPlayerCardDAO playerCardDAO, ISeriesDAO seriesDAO)
        {
            _tournamentDAO = tournamentDAO;
            _tournamentPlayerDAO = tournamentPlayerDAO;
            _gameDAO = gameDAO;
            _userDAO = userDAO;
            _playerCardDAO = playerCardDAO;
            _seriesDAO = seriesDAO;
        }

        public async Task<Tournament> Create(CreateTournamentDTO tournamentDTO, int organizerId)
        {
            //if(tournamentDTO.LocalStartDate < DateTime.UtcNow) 
            //    throw new BadRequestException("La fecha de inicio debe ser mayor a la fecha actual");

            if (tournamentDTO.LocalEndDate <= tournamentDTO.LocalStartDate)
                throw new BadRequestException("la fecha de finalizacion debe ser mayor a la de comienzo");

            // valido que las series esten registradas y las asocio al torneo
            foreach (var seriesId in tournamentDTO.AvailableSeries)
            {
                if(!await _seriesDAO.ExistsSeries(seriesId))
                {
                    throw new NotFoundException("No se encontro la serie de cartas con id: " + seriesId);
                }
            }


            try
            {
                // Asegurar que las fechas sean tratadas como locales
                DateTime localStartDate = DateTime.SpecifyKind(tournamentDTO.LocalStartDate, DateTimeKind.Unspecified);
                DateTime localEndDate = DateTime.SpecifyKind(tournamentDTO.LocalEndDate, DateTimeKind.Unspecified);

                // Obtener la zona horaria
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(tournamentDTO.TimeZoneId);

                // Convertir a UTC
                DateTime startDateUtc = TimeZoneInfo.ConvertTimeToUtc(localStartDate, timeZone);
                DateTime endDateUtc = TimeZoneInfo.ConvertTimeToUtc(localEndDate, timeZone);

                if (startDateUtc < DateTime.UtcNow)
                    throw new BadRequestException("La fecha de inicio debe ser mayor a la fecha actual");

                Tournament tournament = new Tournament
                {
                    Name = tournamentDTO.Name,
                    StartDate = startDateUtc,
                    EndDate = endDateUtc,
                    CountryId = tournamentDTO.CountryId,
                    OrganizerId = organizerId,
                    Phase = TournamentPhase.Registration
                };

                tournament.Id = await _tournamentDAO.CreateAsync(tournament);

                // con el id del torneo creado, asigno las series de cartas al torneo
                foreach (var seriesId in tournamentDTO.AvailableSeries)
                {
                    await _seriesDAO.AssignSeriesToTournament(tournament.Id, seriesId);
                }

                //TimeSpan systemUtcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
                //tournament.StartTime = tournament.StartTime + systemUtcOffset;
                //tournament.EndTime = tournament.EndTime + systemUtcOffset;

                return tournament;

            }
            catch (TimeZoneNotFoundException ex)
            {
                throw new BadRequestException("Zona horaria no definida");
            }
        }

        public async Task<bool> RegisterPlayer(int tournamentId, int playerId, int deckId)
        {
            // el torneo existe y esta en etapa de registro?
            var tournament = await _tournamentDAO.GetByIdAsync(tournamentId);
            if (tournament == null)
                throw new NotFoundException("El torneo no existe");

            if (tournament.Phase != TournamentPhase.Registration)
                throw new RegistrationClosedException("No se permiten mas inscripciones en el torneo");


            // primero reviso que haya cupo
            int maxPlayers = CalculateMaxPlayers(tournament.StartDate, tournament.EndDate);

            List<int> playersRegistered = await _tournamentPlayerDAO.GetTournamentPlayersAsync(tournamentId);
            
            if(playersRegistered.Count() >= maxPlayers)
                throw new RegistrationClosedException("Cupo del torneo completado. No se admiten mas jugadors");

            // validacions del deck
            if (deckId <= 0)
                throw new InvalidDeckException("Debes seleccionar un amzo para el torneo");

            // revisar que el mazo este permitido
            var invalidCards = await _tournamentPlayerDAO.GetInvalidCards(deckId, tournamentId);

            if(invalidCards.Count() > 0)
                throw new InvalidDeckException();

            return await _tournamentPlayerDAO.RegisterPlayerAsync(tournamentId, playerId, deckId);
        }

        public async Task<bool> AssignJudgeToTournament(int tournamentId, int judgeId)
        {
            // el torneo existe y esta en etapa de registro?
            var tournament = await _tournamentDAO.GetByIdAsync(tournamentId);
            if (tournament == null)
                throw new NotFoundException("El torneo no existe");

            //if (tournament.Phase != TournamentPhase.Registration)
            //    throw new RegistrationClosedException("No se permiten mas inscripciones en el torneo");

            var judge = await _userDAO.GetById(judgeId);
            if (judge == null)
                throw new NotFoundException("No se encontro el juez");
            if (judge.Role != UserRole.Judge)
                throw new UnauthorizedRoleException("Solo se permiten asignar jueces");


            return await _tournamentPlayerDAO.RegisterJudgeAsync(tournamentId, judgeId);
        }


        public async Task<bool> AssignSeriesToTournament(int tournamentId, int seriesId)
        {
            // el torneo existe y esta en etapa de registro?
            var tournament = await _tournamentDAO.GetByIdAsync(tournamentId);
            if (tournament == null)
                throw new NotFoundException("El torneo no existe");

            //if (tournament.Phase != TournamentPhase.Registration)
            //    throw new RegistrationClosedException("No se permiten mas inscripciones en el torneo");

            var series = await _seriesDAO.ExistsSeries(seriesId);
            if (!series)
                throw new NotFoundException("La serie seleccionada no se encontro");


            return await _seriesDAO.AssignSeriesToTournament(tournamentId, seriesId);
        }

        public async Task AdvanceTournamentPhase(int tournamentId)
        {
            var tournament = await _tournamentDAO.GetByIdAsync(tournamentId);

            switch (tournament.Phase)
            {
                case TournamentPhase.Registration:
                    await StartTournament(tournament);
                    break;
                case TournamentPhase.InProgress:
                    await ScheduleNextRound(tournament);
                    break;
                case TournamentPhase.Finished:
                    throw new TournamentFinishedException();
                case TournamentPhase.Canceled:
                    throw new TournamentCanceledException();
            }
        }

        private async Task StartTournament(Tournament tournament)
        {
            // priemro revisar que haya lista de jueces y series de cartas habilitadas

            // obtengo los jugadores registrados
            var playersIds = await _tournamentPlayerDAO.GetTournamentPlayersAsync(tournament.Id);

            if (playersIds.Count < 2)
                throw new InvalidOperationException("No hay suficientes jugadores para comenzar el torneo");

            // TODO: validar que haya jueces asignados al torneo

            // Creo una ronda de partidos
            await CreateNextRoundGames(tournament, playersIds);

            tournament.Phase = TournamentPhase.InProgress;
            await _tournamentDAO.UpdateAsync(tournament);
        }

        private async Task ScheduleNextRound(Tournament tournament)
        {
            // Obtengo los jugadores que no estan eliminados del torneo
            var winners = await _tournamentPlayerDAO.GetWinnersAsync(tournament.Id);

            if (winners.Count() <= 1)
            {
                // Queda un solo jugador, se finaliza el torneo
                await FinalizeTournament(tournament, winners.FirstOrDefault());
                return;
            }

            // Create next round games
            await CreateNextRoundGames(tournament, winners);
        }

        private async Task CreateNextRoundGames(Tournament tournament, List<int> players)
        {
            List<Game> games = new List<Game>();

            Random rnd = new Random();
            List<int> playersShuffled = players.OrderBy(x => rnd.Next()).ToList();


            // como manejo la fecha de inicio de cada partida?
            // tomo desde a finalización de la ultima partida?
            // y la primera es la fecha de inicio del torneo?

            var lastGameDate = await _gameDAO.GetLastGameDateAsync(tournament.Id);

            DateTime currentDate;

            if(!lastGameDate.HasValue)
            {
                currentDate = tournament.StartDate;
            } else
            {
                currentDate = lastGameDate.Value;

                if (currentDate.AddMinutes(30).TimeOfDay <= tournament.EndDate.TimeOfDay)
                {
                    currentDate = currentDate.AddMinutes(30);
                }
                else
                {
                    currentDate = currentDate.Date.AddDays(1).Add(tournament.StartDate.TimeOfDay);
                }
            }
            

            if (playersShuffled.Count % 2 != 0)
            {
                int lastPlayer = playersShuffled[^1];
                var game = new Game
                {
                    TournamentId = tournament.Id,
                    Player1Id = lastPlayer,
                    Player2Id = null, // ver si admito valores null para el player 2 o como manejarlo
                    WinnerId = lastPlayer,
                    StartDate = null
                };
                // lo guardo en la BD
                await _gameDAO.Create(game);
                games.Add(game);
            }

            for (int i = 0; i < playersShuffled.Count - 1; i += 2)
            {

                if(currentDate > tournament.EndDate)
                {
                    // sirve verificar esto?
                }

                // crear un Game con 2 jugadores
                var game = new Game
                {
                    TournamentId = tournament.Id,
                    Player1Id = playersShuffled[i],
                    Player2Id = playersShuffled[i + 1],
                    WinnerId = null,
                    StartDate = currentDate,
                };
                // lo guardo en la BD
                await _gameDAO.Create(game);
                games.Add(game);

                if (currentDate.AddMinutes(30).TimeOfDay <= tournament.EndDate.TimeOfDay)
                {
                    currentDate = currentDate.AddMinutes(30);
                } else
                {
                    currentDate = currentDate.Date.AddDays(1).Add(tournament.StartDate.TimeOfDay);
                }
            }

            // revisar por las dudas

        }

        private async Task FinalizeTournament(Tournament tournament, int winnerId)
        {
            tournament.Phase = TournamentPhase.Finished;
            tournament.WinnerId = winnerId;
            await _tournamentDAO.UpdateAsync(tournament);
        }

        public int CalculateMaxPlayers(DateTime start, DateTime end)
        {
            int maxPlayers = 0;
            int games = 0;
            int gameDuration = 30 * 60;

            int dayDuration = 24 * 60 * 60;

            TimeSpan difference = end - start;
            double totalSeconds = difference.TotalSeconds;

            while(totalSeconds > dayDuration)
            {
                totalSeconds -= dayDuration;
            }

            double dayFreeTime = dayDuration - totalSeconds;

            DateTime current = start;
            DateTime endOfDay = start.AddSeconds(totalSeconds);

            while(current <= end)
            {
                while((endOfDay - current).TotalSeconds >= gameDuration)
                {
                    games ++;
                    current = current.AddSeconds(gameDuration);
                }
                current = current.AddSeconds(dayFreeTime);
                endOfDay = endOfDay.AddDays(1);
            }        
            
            maxPlayers = games + 1;

            return maxPlayers;
        }

        public async Task<bool> DisqualifyPlayer(DisqualificationDTO disqualificationDTO, int judgeId)
        {

            // primero chequear que el juez pertenezca al torneo

            // chequeo que el player pertenezca al torneo

            // descalificarlo
            Disqualification disqualification = new Disqualification
            {
                PlayerId = disqualificationDTO.PlayerId,
                TournamentId = disqualificationDTO.TournamentId,
                Reason = disqualificationDTO.Reason,
                JudgeId = judgeId
            };
            
            // ver que pasa si esta en una partida

            return await _tournamentDAO.DisqualifyPlayer(disqualification);
        }
        public async Task CancelTournament(int tournamentId, int adminId)
        {
            var tournament = await _tournamentDAO.GetByIdAsync(tournamentId);

            if(tournament == null)
                throw new NotFoundException("No se encontro el torneo con id " + tournamentId);

            tournament.Phase = TournamentPhase.Canceled;
            tournament.WinnerId = null;
            await _tournamentDAO.UpdateAsync(tournament);
        }


        private async Task<bool> CheckDeckInSeries(int deckId, int seriesId)
        {
            // este metodo debe obtener las cartas del deck y chequear que esten en la serie de cartas permitida
            var cards = await _playerCardDAO.GetCardsByDeckId(deckId);

            foreach (Card card in cards.ToList())
            {

                bool exists = await _seriesDAO.CheckCardInSeries(card.Id, seriesId);
                if (!exists)
                {
                    return false;
                }
            }
            return true;

            // manejarlo en una consulta de sql
        }
        

        public Task<string> DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Tournament>> GetAll()
        {
            var tournaments = await _tournamentDAO.GetAllAsync();


            // aca tengo que convertir las fechas que traigo en utc al horario del usuario logueado
            // o del pais del torneo



            return (List<Tournament>)tournaments;
        }

        public async Task<Tournament> GetById(int id)
        {
            var tournament = await _tournamentDAO.GetByIdAsync(id);
            return tournament;
        }

        public Task<Tournament> Update(Tournament tournament)
        {
            throw new NotImplementedException();
        }



        // metodo para convertir los horarios del torneo en UTC
        private TimeSpan ConvertToUtc(TimeSpan time)
        {
            // Combina el TimeOnly con una fecha arbitraria (ej. 01/01/2000)
            DateTime localDateTime = DateTime.Today.Add(time);

            // Convierte la hora local a UTC
            DateTime utcDateTime = localDateTime.ToUniversalTime();

            // Devuelve solo la hora en UTC como TimeSpan
            return utcDateTime.TimeOfDay;
        }




        //metodo para calcular la ronda en la que va el torneo
        private int GetTournamentRound(int players, int gamesPlayed)
        {
            int round = 0;

            return round;
        }

        // metodo para calcular la cantidad de rondas que tiene un torneo
        private int CalculateTournamentRounds(int players)
        {
            double r = Math.Log(players, 2);

            int rounds = (int)Math.Ceiling(r);

            return rounds;
        }

    }
}
