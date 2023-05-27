using System;

namespace ApiCRUD.Service.Exceptions
{
    public class NotFoundUserException : Exception
    {
        private readonly string _login;
        public override string Message => $"User not found, {_login}";
        public NotFoundUserException() { }
        public NotFoundUserException(string login) => _login = login;
    }
}