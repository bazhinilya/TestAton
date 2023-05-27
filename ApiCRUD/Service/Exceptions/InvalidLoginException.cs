using System;

namespace ApiCRUD.Service.Exceptions
{
    public class InvalidLoginException : Exception
    {
        private readonly string _login;
        public override string Message => $"Invalid login value, {_login}";
        public InvalidLoginException() { }
        public InvalidLoginException(string login) => _login = login;
    }
}