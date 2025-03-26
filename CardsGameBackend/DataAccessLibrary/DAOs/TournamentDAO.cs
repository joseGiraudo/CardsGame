using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLibrary.DAOs.Interface;
using DataAccessLibrary.Exceptions;
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

            try
            {
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
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al crear el torneo: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado  al crear el torneo", ex);
            }
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Tournament>> GetAllAsync()
        {
            string query = "SELECT * FROM tournaments";

            
            try
            {
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
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al obtener los torneos: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al obtener los torneos", ex);
            }
        }

        public async Task<Tournament> GetByIdAsync(int id)
        {
            string query = "SELECT * FROM tournaments WHERE id = @id";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var tournament = await connection.QueryFirstOrDefaultAsync<Tournament>(query, new { id = id });
                    if (tournament == null)
                    {
                        throw new Exception("No se encontro el torneo");
                    }
                    return tournament;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al obtener el torneo: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al obtener el torneo", ex);
            }
        }

        public async Task<bool> GetTournamentJudges(int tournamentId)
        {
            string query = "SELECT COUNT(judgeId) FROM tournament_judges WHERE tournamentId = @TournamentId";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var judges = await connection.ExecuteScalarAsync<int>(query, new { TournamentId = tournamentId });

                    return judges > 0;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al obtener los jueces del torneo: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al obtener los jueces del torneo", ex);
            }
        }

        public async Task UpdateAsync(Tournament tournament)
        {
            string query = "UPDATE tournaments SET phase = @Phase, winnerId = @WinnerId " +
                            "WHERE id = @Id;";
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.ExecuteAsync(query, new
                    {
                        Phase = tournament.Phase.ToString(),
                        WinnerId = tournament.WinnerId,
                        Id = tournament.Id
                    });
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al actualizar el torneo: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al actualizar el torneo", ex);
            }
        }

        public async Task<IEnumerable<Tournament>> GetByPhaseAsync(TournamentPhase phase)
        {
            string query = "SELECT * FROM tournaments WHERE phase = @phase";

            try
            {
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
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al obtener los torneos: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al obtener los torneos", ex);
            }
        }

        public async Task<bool> DisqualifyPlayer(Disqualification disqualification)
        {
            string query = @"INSERT INTO disqualifications
                    (playerId, tournamentId, judgeId, reason) " +
               "VALUES(@PlayerId, @TournamentId, @JudgeId, @Reason); ";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();


                    var affectedRows = await connection.ExecuteScalarAsync<int>(query,
                        new
                        {
                            PlayerId = disqualification.PlayerId,
                            TournamentId = disqualification.TournamentId,
                            JudgeId = disqualification.JudgeId,
                            Reason = disqualification.Reason,
                        });
                    return affectedRows > 0;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al descalificar al jugador: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al descalificar al jugador", ex);
            }
        }

        public async Task<bool> CheckRoundFinished(int tournamentId)
        {
            string query = "SELECT COUNT(id) FROM games WHERE tournamentId = @TournamentId AND winnerId IS NULL;";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var gamesNotFinished = await connection.ExecuteScalarAsync<int>(query, new { TournamentId = tournamentId });

                    return gamesNotFinished == 0;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al obtener los partidos no finalizados: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al obtener los partidos no finalizados", ex);
            }
        }

        public async Task<bool> IsJudgeAvailableInTournament(int gameId, int judgeId)
        {
            string query = @"SELECT EXISTS (
                    SELECT 1
                    FROM tournament_judges tj
                    JOIN games g ON tj.tournamentId = g.tournamentId
                    WHERE g.id = @GameId AND tj.judgeId = @JudgeId
            ) AS isAvailable;";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var isAvailable = await connection.ExecuteScalarAsync<bool>(query, new { GameId = gameId, JudgeId = judgeId });

                    return isAvailable;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al obtener las partidas: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al obtener las partidas", ex);
            }

        }

    }
}
