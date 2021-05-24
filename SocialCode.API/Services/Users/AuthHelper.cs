using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SocialCode.API.Services.Converters;
using SocialCode.API.Services.Requests.Users.Auth;
using SocialCode.Domain.User;

namespace SocialCode.API.Services.Users
{
    public static class AuthHelper
    {
        public static AuthResponse Authenticate(User user, string key)
        {
            if (user is null) return null;
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                }),
                Expires = DateTime.Now.AddHours(2),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            user.Token = tokenHandler.WriteToken(token);
            
            return UserConverter.User_ToAuthResponse(user);
        }
    }
}