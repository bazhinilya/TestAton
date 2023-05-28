using ApiCRUD.Context;
using ApiCRUD.Models.Entities;
using ApiCRUD.Models.Enums;
using ApiCRUD.Service.Exceptions;
using ApiCRUD.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCRUD.Service
{
    public class UserService : IUserService
    {
        private readonly UserContext _context;
        public UserService(UserContext context) => _context = context;

        public async Task<ActionResult<User>> CreateUser(string login, string password, string name, Gender gender, DateTime? birthDay, bool admin)
        {
            try
            {
                if (UserAuthorization.AuthorizedUser.Admin)
                    throw new AccessDeniedException(UserAuthorization.AuthorizedUser.Login);
                var userToCheckByNull = await _context.Users.FirstOrDefaultAsync(u => u.Login.Contains(login));
                if (userToCheckByNull != null)
                    throw new InvalidLoginException(login);
                User userToCreate = new()
                {
                    Guid = Guid.NewGuid(),
                    Login = login,
                    Password = password,
                    Name = name,
                    Gender = gender,
                    BirthDate = birthDay,
                    Admin = admin,
                    CreatedBy = UserAuthorization.AuthorizedUser.Login,
                    CreatedOn = DateTime.Now
                };
                await _context.Users.AddAsync(userToCreate);
                await _context.SaveChangesAsync();
                return userToCreate;
            }
            catch { throw; }
        }

        public async Task<ActionResult<User>> UpdateUser(string login, string fieldToUpdate, string valueToUpdate)
        {
            try
            {
                if (!(UserAuthorization.AuthorizedUser.Admin || UserAuthorization.AuthorizedUser.Login == login))
                    throw new AccessDeniedException(UserAuthorization.AuthorizedUser.Login);
                var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Login.Contains(login));
                if (userToUpdate == null || userToUpdate.RevokedOn != null)
                    throw new NotFoundUserException(login);
                switch (fieldToUpdate)
                {
                    case "Name": 
                        userToUpdate.Name = valueToUpdate;
                        break;
                    case "BirthDate":
                        if (!DateTime.TryParse(valueToUpdate, out DateTime birthDate))
                            throw new Exception("Invalid field value");
                        userToUpdate.BirthDate = birthDate;
                        break;
                    case "Gender":
                        if (int.TryParse(valueToUpdate, out int gender))
                            throw new Exception("Invalid field value");
                        userToUpdate.Gender = (Gender)gender;
                        break;
                    default: 
                        throw new Exception("Invalid field value");
                }
                await _context.SaveChangesAsync();
                return userToUpdate;
            }
            catch { throw; }
        }

        public async Task<ActionResult<User>> UpdatePassword(string login, string password) 
        {
            try
            {
                if (!(UserAuthorization.AuthorizedUser.Admin || UserAuthorization.AuthorizedUser.Login == login))
                    throw new AccessDeniedException(UserAuthorization.AuthorizedUser.Login);
                var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Login.Contains(login));
                if (userToUpdate == null || userToUpdate.RevokedOn != null)
                    throw new NotFoundUserException(login);
                userToUpdate.Password = password;
                await _context.SaveChangesAsync();
                return userToUpdate;
            }
            catch { throw; }
        }

        public async Task<ActionResult<User>> UpdateLogin(string login, string newLogin)
        {
            try
            {
                if (!(UserAuthorization.AuthorizedUser.Admin || UserAuthorization.AuthorizedUser.Login == login))
                    throw new AccessDeniedException(UserAuthorization.AuthorizedUser.Login);
                var userToVerifyLogin = await _context.Users.FirstOrDefaultAsync(u => u.Login.Contains(newLogin));
                if (userToVerifyLogin != null)
                    throw new InvalidLoginException(newLogin);
                var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Login.Contains(login));
                if (userToUpdate == null || userToUpdate.RevokedOn != null)    
                    throw new NotFoundUserException(login);
                userToUpdate.Login = newLogin;
                await _context.SaveChangesAsync();
                return userToUpdate;
            }
            catch { throw; }
        }

        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            try
            {
                if (!UserAuthorization.AuthorizedUser.Admin)
                    throw new AccessDeniedException(UserAuthorization.AuthorizedUser.Login);
                var activeUsers = await _context.Users.Where(u => u.RevokedOn == null)?.OrderBy(u => u.CreatedOn).ToListAsync()
                    ?? throw new NotFoundUserException();
                return activeUsers;
            }
            catch { throw; }
        }

        public async Task<ActionResult<User>> Get(string login)
        {
            try
            {
                if (!UserAuthorization.AuthorizedUser.Admin)
                    throw new AccessDeniedException(UserAuthorization.AuthorizedUser.Login);
                var userByLogin = _context.Users
                        .Where(u => u.Login.Contains(login))
                        ?.Select(u => new User
                        {
                            Name = u.Name,
                            Gender = u.Gender,
                            BirthDate = u.BirthDate,
                            RevokedOn = u.RevokedOn
                        })
                        ?? throw new NotFoundUserException(login);
                return await userByLogin.FirstAsync();
            }
            catch { throw; }
        }

        public ActionResult<User> Get(string login, string password)
        {
            try
            {
                if (!(UserAuthorization.AuthorizedUser.Login.Contains(login) && UserAuthorization.AuthorizedUser.Password == password))
                    throw new AccessDeniedException();
                if (UserAuthorization.AuthorizedUser.RevokedOn != null)
                    throw new NotFoundUserException(login);
                return UserAuthorization.AuthorizedUser;
            }
            catch { throw; }
        }

        public ActionResult<IEnumerable<User>> Get(int age)
        {
            try
            {
                if (!UserAuthorization.AuthorizedUser.Admin)
                    throw new AccessDeniedException(UserAuthorization.AuthorizedUser.Login);
                var usersByAge = _context.Users
                    .AsEnumerable()
                    .Where(u => new DateTime(DateTime.Today.Subtract(u.BirthDate ?? DateTime.Today).Ticks).Year - 1 > age)?
                    .ToList()
                    ?? throw new NotFoundUserException();
                return usersByAge;
            }
            catch { throw; }
        }

        public async Task<ActionResult<User>> DeleteUser(string login, bool isFullDelete = false)
        {
            try
            {
                if (!UserAuthorization.AuthorizedUser.Admin)
                    throw new AccessDeniedException(UserAuthorization.AuthorizedUser.Login);
                var userByLogin = await _context.Users.FirstOrDefaultAsync(u => u.Login.Contains(login)) 
                    ?? throw new NotFoundUserException(login);
                if (isFullDelete)
                {
                    _context.Remove(userByLogin);
                }
                else if (userByLogin.RevokedOn == null)
                {
                    userByLogin.RevokedOn = DateTime.Now;
                    userByLogin.RevokedBy = UserAuthorization.AuthorizedUser.Login;
                }
                else
                {
                    throw new InvalidLoginException(login);
                }
                await _context.SaveChangesAsync();
                return userByLogin;
            }
            catch { throw; }
        }

        public async Task<ActionResult<User>> RecoveryUser(string login) 
        {
            try
            {
                if (!UserAuthorization.AuthorizedUser.Admin)
                    throw new AccessDeniedException(UserAuthorization.AuthorizedUser.Login);
                var userByLogin = await _context.Users.FirstOrDefaultAsync(u => u.Login.Contains(login))
                   ?? throw new NotFoundUserException(login);
                if (userByLogin.RevokedOn == null)
                    throw new InvalidLoginException(login);
                userByLogin.RevokedOn = null;
                userByLogin.RevokedBy = null;
                await _context.SaveChangesAsync();
                return userByLogin;
            }
            catch { throw; }
        }
    }
}