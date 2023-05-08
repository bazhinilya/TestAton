using ApiCRUD.Context;
using ApiCRUD.Models.Entities;
using ApiCRUD.Models.Enums;
using ApiCRUD.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ApiCRUD.Service
{
    public class UserService : IUserService
    {
        private readonly UserContext _context;
        private User _authorizationUser;
        public UserService(UserContext context) => _context = context;
        public bool ValidateCredentials(string userName, string password)
        {
            _authorizationUser = _context.Users.FirstOrDefault(u => u.Login.Contains(userName) && u.Password == password);
            return _authorizationUser != null;
        }

        public async Task<ActionResult<User>> CreateUser(string login, string password, string name, Gender gender, DateTime? birthDay, bool admin)
        {
            try
            {
                if (!_authorizationUser.Admin)
                    throw new Exception("No access rights");
                var userToCheckByNull = await _context.Users.FirstOrDefaultAsync(u => u.Login.Contains(login));
                if (userToCheckByNull != null /*is not null*/)
                    throw new Exception($"User {login} exists");
                User userToCreate = new()
                {
                    Guid = Guid.NewGuid(),
                    Login = login,
                    Password = password,
                    Name = name,
                    Gender = gender,
                    BirthDay = birthDay,
                    Admin = admin,
                    CreatedBy = _authorizationUser.Login,
                    CreatedOn = DateTime.Now
                };
                await _context.Users.AddAsync(userToCreate);
                await _context.SaveChangesAsync();
                return userToCreate;
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult($"InnerError: {ex.Message}");
            }
        }

        public async Task<ActionResult<User>> UpdateUser(string login, string fieldToUpdate)
        {
            try
            {
                if (!(_authorizationUser.Admin || _authorizationUser.Login == login))
                    throw new Exception("No access rights");
                var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Login.Contains(login))
                    ?? throw new Exception($"User {login} doesn't exist");
                if (userToUpdate.RevokedOn != null)
                    throw new Exception($"User's {login} not active");
                if (DateTime.TryParse(fieldToUpdate, out DateTime birthDay)) 
                {
                    userToUpdate.BirthDay = birthDay;
                }
                else if (int.TryParse(fieldToUpdate, out int gender))
                {
                    userToUpdate.Gender = (Gender)gender;
                }
                else
                {
                    userToUpdate.Name = fieldToUpdate;
                }
                await _context.SaveChangesAsync();
                return userToUpdate;
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult($"InnerError: {ex.Message}");
            }
        }

        public async Task<ActionResult<User>> UpdatePassword(string login, string password) 
        {
            try
            {
                if (!(_authorizationUser.Admin || _authorizationUser.Login == login))
                    throw new Exception("No access rights");
                var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Login.Contains(login))
                    ?? throw new Exception($"User {login} doesn't exist");
                if (userToUpdate.RevokedOn != null)
                    throw new Exception($"User's {login} not active");
                userToUpdate.Password = password;
                await _context.SaveChangesAsync();
                return userToUpdate;
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult($"InnerError: {ex.Message}");
            }
        }

        public async Task<ActionResult<User>> UpdateLogin(string login, string newLogin)
        {
            try
            {
                if (!(_authorizationUser.Admin || _authorizationUser.Login == login))
                    throw new Exception("No access rights");
                var userToVerifyLogin = await _context.Users.FirstOrDefaultAsync(u => u.Login.Contains(newLogin));
                if (userToVerifyLogin != null)
                    throw new Exception($"User {newLogin} exists");
                var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Login.Contains(login))
                    ?? throw new Exception($"User {login} doesn't exist");
                if (userToUpdate.RevokedOn != null)
                    throw new Exception($"User's {login} not active");
                userToUpdate.Login = newLogin;
                await _context.SaveChangesAsync();
                return userToUpdate;
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult($"InnerError: {ex.Message}");
            }
        }

        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            try
            {
                if (!_authorizationUser.Admin)
                    throw new Exception("No access rights");
                var activeUsers = await _context.Users.Where(u => u.RevokedOn == null)?.OrderBy(u => u.CreatedOn).ToListAsync()
                    ?? throw new Exception($"Users not found");
                return activeUsers;
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult($"InnerError: {ex.Message}");
            }
        }

        public async Task<ActionResult<User>> Get(string login)
        {
            try
            {
                if (!_authorizationUser.Admin)
                    throw new Exception("No access rights");
                var userByLogin = _context.Users
                        .Where(u => u.Login.Contains(login))
                        ?.Select(u => new User
                        {
                            Name = u.Name,
                            Gender = u.Gender,
                            BirthDay = u.BirthDay,
                            RevokedOn = u.RevokedOn
                        })
                        ?? throw new Exception($"User {login} doesn't exist");
                return await userByLogin.FirstAsync();
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult($"InnerError: {ex.Message}");
            }
        }
        public async Task<ActionResult<User>> Get(string login, string password)
        {
            try
            {
                if (_authorizationUser.RevokedOn != null)
                    throw new Exception($"User's {login} not active");
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Login.Contains(login) && u.Password.Equals(password))
                    ?? throw new Exception($"User {login} doesn't exist");
                return user.Login == _authorizationUser.Login ? user : throw new Exception("No access rights");
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult($"InnerError: {ex.Message}");
            }
        }
        public async Task<ActionResult<IEnumerable<User>>> Get(DateTime age)
        {
            try
            {
                if (!_authorizationUser.Admin)
                    throw new Exception("No access rights");
                var usersByAge = await _context.Users.Where(u => u.BirthDay > age)?.ToListAsync()
                    ?? throw new Exception($"Users not found");
                return usersByAge;
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult($"InnerError: {ex.Message}");
            }
        }

        public async Task<ActionResult<User>> DeleteUser(string login, bool fullDelete = false)
        {
            try
            {
                if (!_authorizationUser.Admin)
                    throw new Exception("No access rights");
                var userByLogin = await _context.Users.FirstOrDefaultAsync(u => u.Login.Contains(login))
                    ?? throw new Exception($"User {login} doesn't exist");
                if (fullDelete)
                {
                    _context.Remove(userByLogin);
                }
                else if (userByLogin.RevokedOn == null)
                {
                    userByLogin.RevokedOn = DateTime.Now;
                    userByLogin.RevokedBy = _authorizationUser.Login;
                }
                else
                {
                    throw new Exception($"User's {login} not active");
                }
                await _context.SaveChangesAsync();
                return userByLogin;
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult($"InnerError: {ex.Message}");
            }
        }

        public async Task<ActionResult<User>> RecoveryUser(string login) 
        {
            try
            {
                if (!_authorizationUser.Admin)
                    throw new Exception("No access rights");
                var userByLogin = await _context.Users.FirstOrDefaultAsync(u => u.Login.Contains(login))
                   ?? throw new Exception($"User {login} doesn't exist");
                if (userByLogin.RevokedOn == null)
                    throw new Exception($"User's {login} active");
                userByLogin.RevokedOn = null;
                userByLogin.RevokedBy = null;
                await _context.SaveChangesAsync();
                return userByLogin;
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult($"InnerError: {ex.Message}");
            }
        }
    }
}