using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs.Interface;
using ModelsLibrary.DTOs.Users;
using ModelsLibrary.Enums;
using ModelsLibrary.Models;
using ServicesLibrary.Exceptions;
using ServicesLibrary.Services.Interface;

namespace ServicesLibrary.Services
{
    public class UserService : IUserService
    {
        private readonly IUserDAO _userDAO;

        public UserService(IUserDAO userDAO)
        {
            _userDAO = userDAO;
        }


        public async Task<User> CreateUser(UserDTO userDTO, int? creatorId, UserRole? creatorRole)
        {

            await ValidateUserCreation(userDTO, creatorId, creatorRole);

            User user = new User()
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                Username = userDTO.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password),
                Avatar = userDTO.Avatar,
                Role = userDTO.Role,
                CountryId = userDTO.CountryId,
                CreatedBy = creatorId > 0 ? creatorId : null,
            };

            int userId = await _userDAO.Create(user);

            user.Id = userId;
            user.Password = null;

            return user;
        }
        
        

        public Task<string> DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<User>> GetAll()
        {
            var users = await _userDAO.GetAll();

            if (users == null || users.Count() < 1)
            {
                throw new NotFoundException("No se encontraron usuarios");
            }

            return (List<User>)users;
        }

        public async Task<User> GetById(int id, int userId, UserRole userRole)
        {
            var user = await _userDAO.GetById(id);

            if (user == null)
            {
                throw new NotFoundException("No se encontro el usuario con id: " + id);
            }

            if ((userRole == UserRole.Player || userRole == UserRole.Judge) && userId != user.Id)
            {
                user.Name = null;
                user.Email = null;
                // ver si así esta bien
            }

            return user;
        }

        public async Task<User> GetById(int id)
        {
            var user = await _userDAO.GetById(id);

            if (user == null)
            {
                throw new NotFoundException("No se encontro el usuario con id: " + id);
            }
            return user;
        }

        public async Task<User> GetByEmail(string email)
        {
            var user = await _userDAO.GetByEmail(email);

            if (user == null)
            {
                throw new NotFoundException("No se encontro el usuario con email: " + email);
            }

            return user;
        }

        public async Task<User> Update(User user)
        {
            if(await _userDAO.Update(user))
            {
                return user;
            }
            throw new NotFoundException("No se encontro el usuario con id: " + user.Id);


        }

        private async Task ValidateUserCreation(UserDTO userDTO, int? creatorId, UserRole? creatorRole)
        {
            // primero valido que tenga el rol necesario para crearlo

            if(creatorId.HasValue && creatorId > 0)
            {
                // hay un creador, entonces 
                switch (creatorRole)
                {
                    case UserRole.Admin:
                        // Puede crear cualquier rol
                        break;

                    case UserRole.Organizer:
                        if (userDTO.Role != UserRole.Judge)
                        {
                            throw new UnauthorizedRoleException("Los organizadores solo pueden crear jueces");
                        }
                        break;

                    default:
                        throw new UnauthorizedRoleException("No tienes permisos para crear usuarios");
                }
            } else
            {
                // si no hay un creador, es un jugador que se quiere registrar solo
                if (userDTO.Role != UserRole.Player)
                {
                    throw new UnauthorizedRoleException("No puedes crear un usuario con ese rol");
                }
            }



            // verificar que no exista un user con ese email

            var user = await GetByEmail(userDTO.Email);

            if (user != null)
                throw new DuplicateEmailException("El email ya se encuentra registrado");


            var userByUsername = await _userDAO.GetByUsername(userDTO.Username);

            if (user != null)
                throw new DuplicateUsernameException("El username ya se encuentra registrado");

        }
    }
}
