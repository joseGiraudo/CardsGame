﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs.Interface;
using ServicesLibrary.Services.Interface;

namespace ServicesLibrary.Services
{
    public class PlayerCardService : IPlayerCardService
    {
        private readonly IPlayerCardDAO _playerCardDAO;

        public PlayerCardService(IPlayerCardDAO playerCardDAO)
        {
            _playerCardDAO = playerCardDAO;
        }

        public async Task<bool> AssignCardToCollection(int cardId, int playerId)
        {
            // tengo que verificar que la carta exista?

            if (await _playerCardDAO.AssignCardToCollection(cardId, playerId))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveCardFromCollection(int cardId, int playerId)
        {
            if (await _playerCardDAO.RemoveCardFromCollection(cardId, playerId))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AssignCardToDeck(int cardId, int deckId)
        {
            if (await _playerCardDAO.GetDeckCardsQuantity(deckId) > 15)
                throw new Exception("No se pueden agregar mas cartas al mazo.");

            if (await _playerCardDAO.AssignCardToDeck(cardId, deckId))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveCardFromDeck(int cardId, int deckId)
        {
            if (await _playerCardDAO.RemoveCardFromDeck(cardId, deckId))
            {
                return true;
            }
            return false;
        }
    }
}
