using ApiCRUD.Context;
using ApiCRUD.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ApiCRUD.Service
{
    internal static class UserAuthorization 
    { 
        internal static User AuthorizedUser { get; set; }
    }

    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly UserContext _context;
        public BasicAuthenticationHandler(UserContext context,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock) =>
                _context = context;

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string userName;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');
                userName = credentials.FirstOrDefault();
                var password = credentials.LastOrDefault();
                if (!ValidateCredentials(userName, password)) 
                    throw new ArgumentException("Invalid credentials");
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName)
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        private bool ValidateCredentials(string userName, string password)
        {
            var authorizationUser = _context.Users.FirstOrDefault(u => u.Login.Contains(userName) && u.Password == password);
            UserAuthorization.AuthorizedUser = authorizationUser;
            return authorizationUser != null;
        }
    }
}