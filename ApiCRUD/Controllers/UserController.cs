using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ApiCRUD.Service;
using ApiCRUD.Context;
using ApiCRUD.Models.Entities;
using ApiCRUD.Models.Enums;

namespace ApiCRUD.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserContext userContext) =>
            _userService = new UserService(userContext);

        #region Create
        [HttpPost]
        public async Task<ActionResult<User>> Create([FromQuery] string login,
            [FromQuery] string password,
            [FromQuery] string name,
            [FromQuery] Gender gender,
            [FromQuery] DateTime? birthDay,
            [FromQuery] bool admin) =>
                await _userService.CreateUser(login, password, name, gender, birthDay, admin);
        #endregion

        #region Update-1
        [HttpPut]
        public async Task<ActionResult<User>> Update([FromQuery] string login, [FromQuery] string fieldToUpdate) =>
            await _userService.UpdateUser(login, fieldToUpdate);

        [HttpPut("2")]
        public async Task<ActionResult<User>> UpdatePassword([FromQuery] string login, [FromQuery] string password) =>
            await _userService.UpdatePassword(login, password);

        [HttpPut("3")]
        public async Task<ActionResult<User>> UpdateLogin([FromQuery] string login, [FromQuery] string newLogin) =>
            await _userService.UpdateLogin(login, newLogin);
        #endregion

        #region Read
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Read() =>
            await _userService.Get();

        [HttpGet("2")]
        public async Task<ActionResult<User>> Read([FromQuery] string login) =>
            await _userService.Get(login);

        [HttpGet("3")]
        public async Task<ActionResult<User>> Read([FromQuery] string login, [FromQuery] string password) =>
            await _userService.Get(login, password);

        [HttpGet("4")]
        public async Task<ActionResult<IEnumerable<User>>> Read([FromQuery] DateTime age) =>
            await _userService.Get(age);
        #endregion

        #region Delete
        [HttpDelete]
        public async Task<ActionResult<User>> Delete([FromQuery] string login, [FromQuery] bool fullDelete = false) =>
            await _userService.DeleteUser(login, fullDelete);
        #endregion

        #region Update-2
        [HttpPut]
        public async Task<ActionResult<User>> Update(string login) =>
            await _userService.RecoveryUser(login);
        #endregion
    }
}
