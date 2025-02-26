using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs.Interface;
using ServicesLibrary.Exceptions;
using ServicesLibrary.Services.Interface;

namespace ServicesLibrary.Services
{
    public class SeriesService : ISeriesService
    {
        private readonly ISeriesDAO _seriesDAO;
        private readonly ICardDAO _cardDAO;

        public SeriesService(ISeriesDAO seriesDAO, ICardDAO cardDAO)
        {
            _seriesDAO = seriesDAO;
            _cardDAO = cardDAO;
        }


        public async Task<bool> AssignCardToSeries(int cardId, int seriesId)
        {
            var card = await _cardDAO.GetById(cardId);
            if (card == null)
                throw new NotFoundException("No se encontro la carta con id: " + cardId);

            // tengo que validar que la carta exista y que la serie exista??????

            return await _seriesDAO.AssignCardToSeries(cardId, seriesId);
        }

        public async Task<bool> CreateCardSeries(string name)
        {
            return await _seriesDAO.CreateCardSeries(name);
        }

        public async Task<bool> RemoveCardFromSeries(int cardId, int seriesId)
        {
            return await _seriesDAO.RemoveCardFromSeries(cardId, seriesId);
        }
    }
}
