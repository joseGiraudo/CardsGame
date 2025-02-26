using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.DAOs.Interface
{
    public interface ISeriesDAO
    {
        public Task<bool> CreateCardSeries(string name);
        public Task<bool> AssignCardToSeries(int cardId, int seriesId);
        public Task<bool> RemoveCardFromSeries(int cardId, int seriesId);
        public Task<bool> ExistsSeries(int seriesId);
        public Task<bool> AssignSeriesToTournament(int tournamentId, int seriesId)

    }
}
