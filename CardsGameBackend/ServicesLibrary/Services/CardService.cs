using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs;
using DataAccessLibrary.DAOs.Interface;
using ModelsLibrary.DTOs.Cards;
using ModelsLibrary.Models;
using ServicesLibrary.Services.Interface;

namespace ServicesLibrary.Services
{
    public class CardService : ICardService
    {
        private readonly ICardDAO _cardDAO;

        public CardService(ICardDAO cardDAO)
        {
            _cardDAO = cardDAO;
        }


        public async Task<int> Create(CardDTO cardDTO)
        {
            Card card = new Card
            {
                Name = cardDTO.Name,
                Attack = cardDTO.Attack,
                Defense = cardDTO.Defense,
                Illustration = cardDTO.Illustration
            };

            return await _cardDAO.Create(card);
        }

        public async Task<bool> DeleteById(int id)
        {
            return await _cardDAO.Delete(id);
        }

        public async Task<List<Card>> GetAll()
        {
            try
            {
                var cards = await _cardDAO.GetAll();

                return (List<Card>) cards;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Card> GetById(int id)
        {
            try
            {
                var card = await _cardDAO.GetById(id);

                return card;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Update(int cardId, CardDTO cardDTO)
        {
            Card card = new Card
            {
                Id = cardId,
                Name = cardDTO.Name,
                Attack = cardDTO.Attack,
                Defense = cardDTO.Defense,
                Illustration = cardDTO.Illustration
            };

            return await _cardDAO.Update(card);
        }
    }
}
