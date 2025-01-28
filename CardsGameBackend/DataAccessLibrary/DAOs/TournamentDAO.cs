using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLibrary.DAOs.Interface;
using Microsoft.Extensions.Configuration;
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


        public async Task<int> Create(Tournament tournament)
        {
            string query = @"INSERT INTO tournaments(name, startDate, endDate, countryId, phase) " +
                "VALUES(@name, @startDate, @endDate, @countryId, @phase); " +
                "SELECT LAST_INSERT_ID;";

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
                        phase = tournament.Phase,
                    });
                return tournamentId;
            }
        }

        public Task<int> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Tournament>> GetAll()
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

                return tournaments;
            }
        }

        public async Task<Tournament> GetById(int id)
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

        public Task<int> Update(Tournament tournament)
        {
            throw new NotImplementedException();
        }
    }
}
