using ApiCRUD.Models.Entities;
using ApiCRUD.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiCRUD.Service.Interfaces
{
    public interface IUserService
    {
        Task<ActionResult<User>> CreateUser(string login, string password, string name, Gender gender, DateTime? birthDay, bool admin);

        Task<ActionResult<User>> UpdateUser(string login, string fieldToUpdate, string valueToUpdate);

        Task<ActionResult<User>> UpdatePassword(string login, string password);

        Task<ActionResult<User>> UpdateLogin(string login, string newLogin);

        Task<ActionResult<IEnumerable<User>>> Get();

        Task<ActionResult<User>> Get(string login);

        ActionResult<User> Get(string login, string password);

        ActionResult<IEnumerable<User>> Get(int age);

        Task<ActionResult<User>> DeleteUser(string login, bool isFullDelete = false);

        Task<ActionResult<User>> RecoveryUser(string login);
    }
}