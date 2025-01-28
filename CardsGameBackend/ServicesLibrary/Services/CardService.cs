using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs;
using DataAccessLibrary.DAOs.Interface;
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


        public async Task<Card> Create(Card card)
        {
            try
            {
                var cardId = await _cardDAO.Create(card);

                return card;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<string> DeleteById(int id)
        {
            throw new NotImplementedException();
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

        public Task<Card> Update(Card card)
        {
            throw new NotImplementedException();
        }
    }
}
