using System;
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
            string query = @"INSERT INTO cards (name, attack, defense, illustration) " +
                            "VALUES(@name, @attack, @defense, @illustration); " +
                            "SELECT LAST_INSERT_ID();";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return await connection.ExecuteScalarAsync<int>(query, new
                    {
                        name = card.Name,
                        attack = card.Attack,
                        defense = card.Defense,
                        illustration = card.Illustration
                    });
                }
            }
            catch (MySqlException ex)
            {
                // Manejar errores específicos de MySQL
                switch (ex.Number)
                {
                    case 1062: // Duplicate entry
                        throw new DatabaseException($"Ya existe una carta con el nombre {card.Name}", ex);
                    case 1452: // Foreign key constraint fails
                        throw new DatabaseException("Error de referencia en la base de datos", ex);
                    case 1042: // Unable to connect
                        throw new DatabaseException("Error de conexión con la base de datos", ex);
                    case 1043: // Bad handshake
                        throw new DatabaseException("Error de conexión con la base de datos", ex);
                    default:
                        throw new DatabaseException($"Error al crear la carta: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                // Otros errores no específicos de MySQL
                throw new DatabaseException("Error inesperado al crear la carta", ex);
            }
        }

        public Task<int> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Card>> GetAll()
        {
            string query = "SELECT * FROM cards";

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

        public async Task<Card> GetById(int id)
        {
            string query = "SELECT * FROM cards WHERE id = @id";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var card = await connection.QueryFirstOrDefaultAsync<Card>(query);
                if (card == null)
                {
                    throw new Exception("Carta no encontrada");
                }

                return card;
            }
        }

        public async Task<int> Update(Card card)
        {
            string query = @"UPDATE cards 
                 SET name = @name, 
                     attack = @attack, 
                     defense = @defense, 
                     illustration = @illustration 
                 WHERE id = @id;";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return await connection.ExecuteAsync(query, new
                {
                    name = card.Name,
                    attack = card.Attack,
                    defense = card.Defense,
                    illustration = card.Illustration,
                    id = card.Id
                });
            }
        }
    }
}
