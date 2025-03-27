using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs.Interface;
using ModelsLibrary.Models;
using ServicesLibrary.Exceptions;
using ServicesLibrary.Services.Interface;

namespace ServicesLibrary.Services
{
    public class SeriesService : ISeriesService
    {
        private readonly ISeriesDAO _seriesDAO;
        private readonly ICardDAO _cardDAO;
        private readonly ITournamentDAO _tournamentDAO;

        public SeriesService(ISeriesDAO seriesDAO, ICardDAO cardDAO, ITournamentDAO tournamentDAO)
        {
            _seriesDAO = seriesDAO;
            _cardDAO = cardDAO;
            _tournamentDAO = tournamentDAO;
        }


        public async Task<bool> AssignCardToSeries(int cardId, int seriesId)
        {
            var card = await _cardDAO.GetById(cardId);
            if (card == null)
                throw new NotFoundException("No se encontro la carta con id: " + cardId);


            return await _seriesDAO.AssignCardToSeries(cardId, seriesId);
        }

        public async Task<bool> AssignSeriesToTournament(int tournamentId, List<int> seriesIds)
        {
            var tournament = await _tournamentDAO.GetByIdAsync(tournamentId);
            if (tournament == null)
                throw new NotFoundException("No se encontro el torneo con id: " + tournamentId);

            foreach(var serieId in seriesIds)
            {
                await _seriesDAO.AssignSeriesToTournament(tournamentId, serieId);
            }
            return true;
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
