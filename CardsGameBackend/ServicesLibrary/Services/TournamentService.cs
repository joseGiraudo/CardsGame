﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs;
using DataAccessLibrary.DAOs.Interface;
using ModelsLibrary.DTOs.Tournament;
using ModelsLibrary.Enums;
using ModelsLibrary.Models;
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

        public TournamentService(ITournamentDAO tournamentDAO, ITournamentPlayerDAO tournamentPlayerDAO,
            IGameDAO gameDAO, IUserDAO userDAO, IPlayerCardDAO playerCardDAO)
        {
            _tournamentDAO = tournamentDAO;
            _tournamentPlayerDAO = tournamentPlayerDAO;
            _gameDAO = gameDAO;
            _userDAO = userDAO;
            _playerCardDAO = playerCardDAO;
        }

        public async Task<Tournament> Create(CreateTournamentDTO tournamentDTO)
        {
            //if(tournamentDTO.LocalStartDate < DateTime.UtcNow) 
            //    throw new BadRequestException("La fecha de inicio debe ser mayor a la fecha actual");

            if (tournamentDTO.LocalEndDate <= tournamentDTO.LocalStartDate)
                throw new BadRequestException("la fecha de finalizacion debe ser mayor a la de comienzo");

            // ver que otras validaciones hay

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

                Tournament tournament = new Tournament
                {
                    Name = tournamentDTO.Name,
                    StartDate = startDateUtc,
                    EndDate = endDateUtc,
                    CountryId = tournamentDTO.CountryId,
                    OrganizerId = tournamentDTO.OrganizerId,
                    Phase = TournamentPhase.Registration
                };

                tournament.Id = await _tournamentDAO.CreateAsync(tournament);

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
            int maxPlayers = CalculateMaxPlayersAsync(tournament.StartDate, tournament.EndDate);

            List<TournamentPlayer> playersRegistered = await _tournamentPlayerDAO.GetTournamentPlayersAsync(tournamentId);
            
            if(playersRegistered.Count() >= maxPlayers)
                throw new RegistrationClosedException("Cupo del torneo completado. No se admiten mas jugadors");



            return await _tournamentPlayerDAO.RegisterPlayerAsync(tournamentId, playerId, deckId);

            // conviene devolver un boolean para saber si se registro?
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

            // conviene devolver un boolean para saber si se registro?
        }


        public async Task<bool> AssignSeriesToTournament(int tournamentId, int seriesId)
        {
            // el torneo existe y esta en etapa de registro?
            var tournament = await _tournamentDAO.GetByIdAsync(tournamentId);
            if (tournament == null)
                throw new NotFoundException("El torneo no existe");

            //if (tournament.Phase != TournamentPhase.Registration)
            //    throw new RegistrationClosedException("No se permiten mas inscripciones en el torneo");

            var series = await _playerCardDAO.ExistsSeries(seriesId);
            if (!series)
                throw new NotFoundException("La serie seleccionada no se encontro");


            return await _playerCardDAO.AssignSeriesToTournament(tournamentId, seriesId);

            // conviene devolver un boolean para saber si se registro?
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
            }
        }

        private async Task StartTournament(Tournament tournament)
        {
            // priemro revisar que haya lista de jueces y series de cartas habilitadas

            // obtengo los jugadores registrados
            var players = await _tournamentPlayerDAO.GetTournamentPlayersAsync(tournament.Id);

            if (players.Count < 2)
                throw new InvalidOperationException("No hay suficientes jugadores para comenzar el torneo");

            // Creo una ronda de partidos
            await CreateNextRoundGames(tournament, players);

            tournament.Phase = TournamentPhase.InProgress;
            await _tournamentDAO.UpdateAsync(tournament);
        }

        private async Task ScheduleNextRound(Tournament tournament)
        {
            // Obtengo los jugadores que no estan eliminados del torneo
            var winners = await _tournamentPlayerDAO.GetRoundWinnersAsync(tournament.Id);

            if (winners.Count() <= 1)
            {
                // Queda un solo jugador, se finaliza el torneo
                await FinalizeTournament(tournament, winners.FirstOrDefault().PlayerId);
                return;
            }

            // Create next round games
            await CreateNextRoundGames(tournament, winners);
        }

        private async Task CreateNextRoundGames(Tournament tournament, List<TournamentPlayer> players)
        {
            // ver si sirve desordenar la lista

            List<Game> games = new List<Game>();

            if(players.Count % 2 != 0)
            {
                TournamentPlayer lastPlayer = players.Last();
                var game = new Game
                {
                    TournamentId = tournament.Id,
                    Player1Id = lastPlayer.PlayerId,
                    Player2Id = lastPlayer.PlayerId, // ver si admito valores null para el player 2 o como manejarlo
                    WinnerId = lastPlayer.PlayerId // dejarlo null y que el juez a mano lo de como ganador
                    // falta el start date
                };
                // lo guardo en la BD
                await _gameDAO.Create(game);
                games.Add(game);
            }

            for (int i = 0; i < players.Count - 1; i += 2)
            {
                // crear un Game con 2 jugadores
                var game = new Game
                {
                    TournamentId = tournament.Id,
                    Player1Id = players[i].PlayerId,
                    Player2Id = players[i + 1].PlayerId,
                    WinnerId = null
                    // falta el start date
                };
                // lo guardo en la BD
                await _gameDAO.Create(game);
                games.Add(game);
            }

            // revisar por las dudas

        }

        private async Task FinalizeTournament(Tournament tournament, int winnerId)
        {
            tournament.Phase = TournamentPhase.Finished;
            tournament.WinnerId = winnerId;
            await _tournamentDAO.UpdateAsync(tournament);
        }

        public int CalculateMaxPlayersAsync(DateTime start, DateTime end)
        {
            int maxPlayers = 0;
            int games = 0;
            int gameDurationMinutes = 30;

            DateTime current = start;

            if (start.TimeOfDay < end.TimeOfDay)
            {
                while (current <= end)
                {
                    double availableMinutes = (end.TimeOfDay - start.TimeOfDay).TotalMinutes;
                    games +=  (int)(availableMinutes / gameDurationMinutes);
                    current = current.AddDays(1);
                }
            }
            else
            {
                while (current <= end)
                {
                    if (current.Date.DayOfYear == start.Date.DayOfYear)
                    {
                        // priemr día
                        double availableMinutes = ((24 * 60) - start.TimeOfDay.TotalMinutes);
                        games += (int)(availableMinutes / gameDurationMinutes);
                        current = current.AddHours(24 - start.TimeOfDay.TotalHours + end.TimeOfDay.TotalHours); // avanzo hasta el horario de inicio del dia siguiente

                    }
                    else if (current.Date.DayOfYear == end.Date.DayOfYear)
                    {
                        // ultimo día
                        double availableMinutes = end.TimeOfDay.TotalMinutes;
                        games += (int)(availableMinutes / gameDurationMinutes);
                        current = current.AddDays(1);
                    } else
                    {
                        // resto de días sumo ambas partes
                        double availableMinutes = (24 * 60) - start.TimeOfDay.TotalMinutes + end.TimeOfDay.TotalMinutes;
                        games += (int)(availableMinutes / gameDurationMinutes);
                        current = current.AddDays(1);
                    }
                }
            }           
            maxPlayers = games - 1;

            return maxPlayers;
        }

        public int CalculateMaxPlayersAsync2(DateTime start, DateTime end)
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
            
            maxPlayers = games - 1;

            return maxPlayers;
        }


        private async Task CheckDeck(int deckId)
        {
            // este metodo debe obtener las cartas del deck y chequear que esten en la serie de cartas permitida
            
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

    }
}
