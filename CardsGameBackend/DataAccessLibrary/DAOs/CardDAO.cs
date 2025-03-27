using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLibrary.DAOs.Interface;
using DataAccessLibrary.Exceptions;
using Microsoft.Extensions.Configuration;
using ModelsLibrary.Models;
using MySqlConnector;

namespace DataAccessLibrary.DAOs
{
    public class CardDAO : ICardDAO
    {
        private readonly string _connectionString;

        public CardDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public async Task<int> Create(Card card)
        {
            string query = @"INSERT INTO cards (name, attack, defense, illustration)
                            VALUES(@name, @attack, @defense, @illustration);
                            SELECT LAST_INSERT_ID();";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    int cardId = await connection.ExecuteScalarAsync<int>(query, new
                    {
                        name = card.Name,
                        attack = card.Attack,
                        defense = card.Defense,
                        illustration = card.Illustration
                    });

                    return cardId;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al crear la carta: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Otros errores no específicos de MySQL
                throw new DatabaseException("Error inesperado al crear la carta", ex);
            }
        }

        public async Task<bool> Delete(int id)
        {
            string query = "DELETE FROM cards WHERE id = @id;";
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    int rowsAffected = await connection.ExecuteAsync(query, new { id });
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al eliminar la carta: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al eliminar la carta", ex);
            }
        }

        public async Task<IEnumerable<Card>> GetAll()
        {
            string query = "SELECT * FROM cards";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var cards = await connection.QueryAsync<Card>(query);
                    if (cards == null || cards.Count() < 1)
                    {
                        throw new Exception("Cartas no encontrados");
                    }

                    return cards;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error inesperado al obtener las cartas: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al obtener las cartas", ex);
            }
        }

        public async Task<Card> GetById(int id)
        {
            string query = "SELECT * FROM cards WHERE id = @id";

            
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var card = await connection.QueryFirstOrDefaultAsync<Card>(query, new {id});

                    return card;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al obtener la carta: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al obtener la carta", ex);
            }
        }

        public async Task<bool> Update(Card card)
        {
            string query = @"UPDATE cards 
                 SET name = @name, 
                     attack = @attack, 
                     defense = @defense, 
                     illustration = @illustration 
                 WHERE id = @id;";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int rowsAffected = await connection.ExecuteAsync(query, new
                    {
                        name = card.Name,
                        attack = card.Attack,
                        defense = card.Defense,
                        illustration = card.Illustration,
                        id = card.Id
                    });

                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al actualizar la carta: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al actualizar la carta", ex);
            }
        }
    }
}
