using System;

namespace ApiCRUD.Service.Exceptions
{
    public class AccessDeniedException : Exception
    {
        private readonly string _login;
        public override string Message => $"No access rights, {_login}";
        public AccessDeniedException() { }
        public AccessDeniedException(string login) => _login = login;
    }
}