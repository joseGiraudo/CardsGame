using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.DAOs.Interface
{
    public interface IPlayerCardDAO
    {
        public Task<bool> AssignCardToCollection(int cardId, int playerId);
        public Task<bool> RemoveCardFromCollection(int cardId, int playerId);
        public Task<bool> AssignCardToDeck(int cardId, int deckId);
        public Task<bool> RemoveCardFromDeck(int cardId, int deckId);
        public Task<bool> AssignCardToSeries(int cardId, int seriesId);
        public Task<bool> RemoveCardFromSeries(int cardId, int seriesId);
        public Task<bool> CreateCardSeries(string name);
        public Task<bool> ExistsSeries(int seriesId);
        public Task<bool> AssignSeriesToTournament(int tournamentId, int seriesId);
    }
}
