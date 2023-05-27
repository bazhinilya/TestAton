using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ApiCRUD.Service;
using ApiCRUD.Context;
using ApiCRUD.Models.Entities;
using ApiCRUD.Models.Enums;
using ApiCRUD.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ApiCRUD.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(UserContext userContext) => _userService = new UserService(userContext);

        #region Create
        /// <summary>
        /// Создает пользователя.
        /// </summary>
        [HttpPost("CreateUser")]
        public async Task<ActionResult<User>> Create([FromQuery] string login,
            [FromQuery] string password,
            [FromQuery] string name,
            [FromQuery] Gender gender,
            [FromQuery] DateTime? birthDay,
            [FromQuery] bool admin) =>
                Ok(await _userService.CreateUser(login, password, name, gender, birthDay, admin));
        #endregion

        #region Update-1
        /// <summary>
        /// Изменение имени, пола или даты рождения пользователя.
        /// </summary>
        [HttpPut("Update-1")]
        public async Task<ActionResult<User>> Update([FromQuery] string login, 
            [FromQuery] string fieldToUpdate, 
            [FromQuery] string valueToUpdate) =>
                Ok(await _userService.UpdateUser(login, fieldToUpdate, valueToUpdate));

        /// <summary>
        /// Изменение пароля.
        /// </summary>
        [HttpPut("Update-1/UpdatePassword")]
        public async Task<ActionResult<User>> UpdatePassword([FromQuery] string login, [FromQuery] string password) =>
            Ok(await _userService.UpdatePassword(login, password));

        /// <summary>
        /// Изменение логина.
        /// </summary>
        [HttpPut("Update-1/UpdateLogin")]
        public async Task<ActionResult<User>> UpdateLogin([FromQuery] string login, [FromQuery] string newLogin) =>
            Ok(await _userService.UpdateLogin(login, newLogin));
        #endregion

        #region Read
        /// <summary>
        /// Запрос списка всех активных пользователей.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Read() =>
            await _userService.Get();

        /// <summary>
        /// Запрос пользователя по логину.
        /// </summary>
        [HttpGet("ReadByLogin/{login}")]
        public async Task<ActionResult<User>> Read(string login) =>
            Ok(await _userService.Get(login));

        /// <summary>
        /// Запрос пользователя по логину и паролю.
        /// </summary>
        [HttpGet("ReadByLoginAndPassword")]
        public ActionResult<User> Read([FromQuery] string login, [FromQuery] string password) =>
            Ok(_userService.Get(login, password));

        /// <summary>
        /// Запрос всех пользователей старше определённого возраста. 
        /// </summary>
        [HttpGet("ReadByAge")]
        public async Task<ActionResult<IEnumerable<User>>> Read(int age) =>
            Ok(await _userService.Get(age));
        #endregion

        #region Delete
        /// <summary>
        /// Удаляет пользователя.
        /// </summary>
        [HttpDelete]
        public async Task<ActionResult<User>> Delete([FromQuery] string login, [FromQuery] bool isFullDelete = false) =>
            Ok(await _userService.DeleteUser(login, isFullDelete));
        #endregion

        #region Update-2
        /// <summary>
        /// Восстанавливает пользователя после мягкого удаления.
        /// </summary>
        [HttpPut("Update-2/{login}")]
        public async Task<ActionResult<User>> Update(string login) =>
            Ok(await _userService.RecoveryUser(login));
        #endregion
    }
}