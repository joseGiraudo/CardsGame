﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLibrary.DAOs.Interface;
using DataAccessLibrary.Exceptions;
using Microsoft.Extensions.Configuration;
using ModelsLibrary.Models;
using MySql.Data.MySqlClient;

namespace DataAccessLibrary.DAOs
{
    public class GameDAO : IGameDAO
    {
        private readonly string _connectionString;

        public GameDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public async Task<int> Create(Game game)
        {
            string query = @"INSERT INTO games (tournamentId, startDate, player1, player2, winnerId) " +
                " VALUES (@tournamentId, @startDate, @player1, @player2, @winnerId);" +
                " SELECT LAST_INSERT_ID();";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var gameId = await connection.ExecuteScalarAsync<int>(query, new
                    {
                        tournamentId = game.TournamentId,
                        startDate = game.Player2 == null ? (DateTime?)null : game.StartDate, // No asigna fecha a byes
                        player1 = game.Player1,
                        player2 = game.Player2,
                        winnerId = game.Player2 == null ? game.Player1 : (int?)null // Si es bye, gana automáticamente
                    });

                    return gameId;
                }
            }
            catch (MySqlException ex)   
            {
                throw new DatabaseException($"Error al crear la partida: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al crear la partida", ex);
            }
        }

        public Task<int> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Game>> GetAll()
        {
            string query = @"SELECT * FROM games";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var games = await connection.QueryAsync<Game>(query);

                    if (games == null || games.Count() == 0)
                    {
                        throw new Exception("No se encontraron partidas");
                    }
                    return games;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al obtener las partidas: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al  obtener las partidas", ex);
            }
        }

        public async Task<Game> GetById(int id)
        {
            string query = @"SELECT * FROM games WHERE id = @id";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var game = await connection.QueryFirstOrDefaultAsync<Game>(query, new { id });

                    if (game == null)
                    {
                        throw new Exception("Partida no encontrada");
                    }
                    return game;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al obtener la partida: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al obtener la partida", ex);
            }
        }

        public async Task<IEnumerable<Game>> GetByTournamentId(int tournamentId)
        {
            string query = @"SELECT * FROM games WHERE tournamentId = @id";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var games = await connection.QueryAsync<Game>(query, new { id = tournamentId });

                    if (games == null || games.Count() == 0)
                    {
                        throw new Exception("Partidas no encontradas para ese torneo");
                    }
                    return games;
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

        public async Task<bool> SetGameWinner(int gameId, int winnerId)
        {
            string query = @"UPDATE games SET winnerId = @WinnerId WHERE id = @GameId;";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int rowsAffected = await connection.ExecuteAsync(query, new
                    {
                        WinnerId = winnerId,
                        GameId  = gameId
                    });
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error de base de datos: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Error inesperado: {ex.Message}", ex);
            }
        }

        public async Task<DateTime?> GetLastGameDateAsync(int tournamentId)
        {
            string query = "SELECT MAX(startDate) FROM games WHERE tournamentId = @TournamentId;";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var lastDate = await connection.QueryFirstOrDefaultAsync<DateTime?>(query, new { TournamentId = tournamentId });

                    if (lastDate == null)
                    {
                        return null;
                    }

                    return lastDate;
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

        public Task<int> Update(Game game)
        {
            throw new NotImplementedException();
        }
    }
}
