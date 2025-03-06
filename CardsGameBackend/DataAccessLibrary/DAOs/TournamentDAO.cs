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
