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
using ServicesLibrary.Exceptions;
using ServicesLibrary.Services.Interface;

namespace ServicesLibrary.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly ITournamentDAO _tournamentDAO;
        private readonly ITournamentPlayerDAO _tournamentPlayerDAO;
        private readonly IGameDAO _gameDAO;

        public TournamentService(ITournamentDAO tournamentDAO, ITournamentPlayerDAO tournamentPlayerDAO, IGameDAO gameDAO)
        {
            _tournamentDAO = tournamentDAO;
            _tournamentPlayerDAO = tournamentPlayerDAO;
            _gameDAO = gameDAO;
        }

        public async Task<Tournament> Create(CreateTournamentDTO tournamentDTO)
        {
            if(tournamentDTO.StartDate < DateTime.UtcNow) 
                throw new BadRequestException("La fecha de inicio debe ser mayor a la fecha actual");

            if (tournamentDTO.EndDate <= tournamentDTO.StartDate)
                throw new BadRequestException("la fecha de finalizacion debe ser mayor a la de comienzo");

            // ver que otras validaciones hay

            Tournament tournament = new Tournament
            {
                Name = tournamentDTO.Name,
                StartDate = tournamentDTO.StartDate,
                EndDate = tournamentDTO.EndDate,
                CountryId = tournamentDTO.CountryId,
                OrganizerId = tournamentDTO.OrganizerId,
                Phase = TournamentPhase.Registration
            };

            tournament.Id = await _tournamentDAO.CreateAsync(tournament);

            return tournament;
        }

        public async Task RegisterPlayer(int tournamentId, int playerId, int deckId)
        {
            // el torneo existe y esta en etapa de registro?
            var tournament = await _tournamentDAO.GetByIdAsync(tournamentId);
            if (tournament == null)
                throw new NotFoundException("El torneo no existe");

            if (tournament.Phase != TournamentPhase.Registration)
                throw new RegistrationClosedException("No se permiten mas inscripciones en el torneo");


            // primero reviso que haya cupo
            int maxPlayers = await CalculateMaxPlayersAsync(tournamentId);

            List<TournamentPlayer> playersRegistered = await _tournamentPlayerDAO.GetTournamentPlayersAsync(tournamentId);
            
            if(playersRegistered.Count() >= maxPlayers)
                throw new RegistrationClosedException("Cupo del torneo completado. No se admiten mas jugadors");



            await _tournamentPlayerDAO.RegisterPlayerAsync(tournamentId, playerId, deckId);

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
            // Get winners from previous round
            var winners = await _tournamentPlayerDAO.GetRoundWinnersAsync(tournament.Id);

            if (winners.Count() <= 1)
            {
                // Final round or tournament complete
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
                    WinnerId = lastPlayer.PlayerId
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
        }

        private async Task FinalizeTournament(Tournament tournament, int winnerId)
        {
            tournament.Phase = TournamentPhase.Finished;
            tournament.WinnerId = winnerId;
            await _tournamentDAO.UpdateAsync(tournament);
        }

        private async Task<int> CalculateMaxPlayersAsync(int tournamentId)
        {
            int maxPlayers = 0;
            
            // ver como tengo las fechas del torneo

            Tournament tournament = await _tournamentDAO.GetByIdAsync(tournamentId);

            int duration = (tournament.EndDate - tournament.StartDate).Days;


            // por ahora supongo que se juegan 8 partidos por dia

            maxPlayers = duration * 8;

            return maxPlayers;
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
    }
}
