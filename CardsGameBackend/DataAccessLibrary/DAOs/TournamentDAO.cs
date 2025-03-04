using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLibrary.DAOs.Interface;
using Microsoft.Extensions.Configuration;
using ModelsLibrary.Enums;
using ModelsLibrary.Models;
using MySqlConnector;

namespace DataAccessLibrary.DAOs
{
    public class TournamentDAO : ITournamentDAO
    {
        private readonly string _connectionString;

        public TournamentDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }


        public async Task<int> CreateAsync(Tournament tournament)
        {
            string query = @"INSERT INTO tournaments
                    (name, startDate, endDate, countryId, phase, organizerId) " +
                "VALUES(@name, @startDate, @endDate, @countryId, @phase, @organizerId); " +
                "SELECT LAST_INSERT_ID();";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();


                var tournamentId = await connection.ExecuteScalarAsync<int>(query,
                    new
                    {
                        name = tournament.Name,
                        startDate = tournament.StartDate,
                        endDate = tournament.EndDate,
                        countryId = tournament.CountryId,
                        phase = tournament.Phase.ToString(),
                        organizerId = tournament.OrganizerId,
                    });
                return tournamentId;
            }
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Tournament>> GetAllAsync()
        {
            string query = "SELECT * FROM tournaments";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var tournaments = await connection.QueryAsync<Tournament>(query);
                if (tournaments == null || tournaments.Count() < 1)
                {
                    throw new Exception("No se encontraron torneos");
                }
                TimeSpan systemUtcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
                foreach (var tournament in tournaments)
                {
                }

                return tournaments;
            }
        }

        public async Task<Tournament> GetByIdAsync(int id)
        {
            string query = "SELECT * FROM tournaments WHERE id = @id";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var tournament = await connection.QueryFirstOrDefaultAsync<Tournament>(query, new {id = id});
                if (tournament == null)
                {
                    throw new Exception("No se encontro el torneo");
                }
                return tournament;
            }
        }

        public async Task UpdateAsync(Tournament tournament)
        {
            string query = "UPDATE tournaments SET phase = @Phase, winnerId = @WinnerId " +
                            "WHERE id = @Id;";
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(query, new
                {
                    Phase = tournament.Phase,
                    WinnerId = tournament.WinnerId,
                    Id = tournament.Id
                });
            }
        }

        public async Task<IEnumerable<Tournament>> GetByPhaseAsync(TournamentPhase phase)
        {
            string query = "SELECT * FROM tournaments WHERE phase = @phase";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var tournaments = await connection.QueryAsync<Tournament>(query, new { phase = phase });
                if (tournaments == null || tournaments.Count() < 1)
                {
                    throw new Exception("No se encontraron torneos");
                }

                return tournaments;
            }
        }

        public async Task<bool> CheckCardsSeries(int deckId, int tournamentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                SELECT 
                    COUNT(dc.cardId) as TotalCards,
                    SUM(CASE WHEN cs.cardId IS NOT NULL THEN 1 ELSE 0 END) as ValidCards
                FROM 
                    decks_cards dc
                    LEFT JOIN (
                        SELECT DISTINCT cs.cardId
                        FROM cards_series cs
                        JOIN tournament_series ts ON cs.seriesId = ts.seriesId
                        WHERE ts.tournamentId = @tournamentId
                    ) cs ON dc.cardId = cs.cardId
                WHERE 
                    dc.deckId = @deckId";

                var result = await connection.QuerySingleAsync(query, new { deckId, tournamentId });

                return result.TotalCards == result.ValidCards;
            }
        }


        public async Task<IEnumerable<Card>> GetInvalidCards(int deckId, int tournamentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                SELECT 
                    c.id, 
                    c.name,
                    c.attack,
                    c.defense,
                    c.illustration
                FROM 
                    decks_cards dc
                    JOIN cards c ON dc.cardId = c.id
                    LEFT JOIN (
                        SELECT DISTINCT cs.cardId
                        FROM cards_series cs
                        JOIN tournament_series ts ON cs.seriesId = ts.seriesId
                        WHERE ts.tournamentId = @tournamentId
                    ) available_cards ON dc.cardId = available_cards.cardId
                WHERE 
                    dc.deckId = @deckId
                    AND available_cards.cardId IS NULL";

                var parameters = new { deckId, tournamentId };
                return await connection.QueryAsync<Card>(query, parameters);
            }
        }

        // metodo para convertir los horarios del torneo en UTC
        private TimeSpan ConvertToUtcTimeOnly(TimeOnly time)
        {
            // Combina el TimeOnly con una fecha arbitraria (ej. 01/01/2000)
            DateTime localDateTime = DateTime.Today.Add(time.ToTimeSpan());

            // Convierte la hora local a UTC
            DateTime utcDateTime = localDateTime.ToUniversalTime();

            // Devuelve solo la hora en UTC como TimeSpan
            return utcDateTime.TimeOfDay;
        }
    }
}
