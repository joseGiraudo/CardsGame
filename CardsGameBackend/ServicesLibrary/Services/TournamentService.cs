using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs.Interface;
using ModelsLibrary.DTOs.Tournament;
using ModelsLibrary.Models;
using ServicesLibrary.Services.Interface;

namespace ServicesLibrary.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly ITournamentDAO _tournamentDAO;

        public TournamentService(ITournamentDAO tournamentDAO)
        {
            _tournamentDAO = tournamentDAO;
        }

        public async Task<Tournament> Create(CreateTournamentDTO tournamentDTO)
        {
            if (tournamentDTO.EndDate > tournamentDTO.StartDate)
            {
                throw new Exception("la fecha de finalizacion no debe ser mayor a la de comienzo");
            }
            // ver que otras validaciones hay
            Tournament tournament = new Tournament();
            tournament.Name = tournamentDTO.Name;
            tournament.StartDate = tournamentDTO.StartDate;
            tournament.EndDate = tournamentDTO.EndDate;
            tournament.CountryId = tournamentDTO.CountryId;

            await _tournamentDAO.Create(tournament);

            return tournament;
        }

        public Task<string> DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Tournament>> GetAll()
        {
            var tournaments = await _tournamentDAO.GetAll();

            return (List<Tournament>)tournaments;
        }

        public async Task<Tournament> GetById(int id)
        {
            var tournament = await _tournamentDAO.GetById(id);
            return tournament;
        }

        public Task<Tournament> Update(Tournament tournament)
        {
            throw new NotImplementedException();
        }
    }
}
