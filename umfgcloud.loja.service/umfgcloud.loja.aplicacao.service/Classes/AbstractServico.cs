using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace umfgcloud.loja.aplicacao.service.Classes
{
    public abstract class AbstractServico
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected string UserId { get; private set; } = string.Empty;
        protected string UserEmail { get; private set; } = string.Empty;

        private string Token { get; set; } = string.Empty;
        private JwtSecurityToken? UserSecurityToken { get; set; } = null;

        protected AbstractServico(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = 
                httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            Token = GetUserToken();
            UserSecurityToken = GetUserSecurityToken();
            UserId = GetUserId();
            UserEmail = GetUserEmail();
        }

        private string GetUserToken()
            => _httpContextAccessor
            ?.HttpContext
            ?.Request
            .Headers[HeaderNames.Authorization]
            .ToString()
            .Split(" ")
            .LastOrDefault() ?? string.Empty;

        private JwtSecurityToken GetUserSecurityToken()
            => new JwtSecurityToken(Token);

        private string GetUserId()
            => IsPayloadContainsKey(ClaimTypes.NameIdentifier)
            ? GetPayloadValue(ClaimTypes.NameIdentifier)
            : throw new InvalidDataException("Usuario não possui um id valido.");

        private string GetUserEmail()
            => IsPayloadContainsKey(ClaimTypes.Email)
            ? GetPayloadValue(ClaimTypes.Email)
            : throw new InvalidDataException("Usuario não possui um email valido.");

        private bool IsPayloadContainsKey(string key)
            => UserSecurityToken?.Payload.ContainsKey(key) ?? false;

        private string GetPayloadValue(string key)
            => UserSecurityToken?.Payload[key].ToString() ?? string.Empty;
    }
}