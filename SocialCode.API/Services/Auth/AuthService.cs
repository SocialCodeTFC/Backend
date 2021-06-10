using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SocialCode.API.Converters;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Auth;
using SocialCode.API.Validators;
using SocialCode.API.Validators.Auth;
using SocialCode.Domain.User;

namespace SocialCode.API.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        public async Task<SocialCodeResult<AuthResponse>> LogIn(LoginRequest loginRequest)
        {
            var scResult = new SocialCodeResult<AuthResponse>();

            //Check if is valid request 
            if (!AuthRequestValidator.IsValidLoginRequest(loginRequest))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Login request is not valid";
                return scResult;
            }

            //Get user from Db
            var user = await _userRepository.GetUserByUsername(loginRequest.Username);

            if (user is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                scResult.ErrorMsg = "User not found!";
                return scResult;
            }

            if (!PasswordUtils.IsValidPassword(user.Password, loginRequest.Password))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Passwords do not match";
                return scResult;
            }

            //Generate & Verify Tokens
            var tokens = GenerateNewTokens(user);
            if (tokens is null || tokens.FirstOrDefault() is null || tokens.LastOrDefault() is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                scResult.ErrorMsg = "Failed creating user tokens";
                return scResult;
            }

            //Update user tokens
            user.Token = tokens.FirstOrDefault();
            user.RefreshToken = tokens.LastOrDefault();

            //Persist new user Tokens in MongoDB
            var updatedUser = await _userRepository.ModifyUser(user.Id, user);
            if (updatedUser is null)
            {
                scResult.ErrorMsg = "Failed to update user Tokens";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                return scResult;
            }

            //Return dto with token & refresh token
            scResult.Value = new AuthResponse
            {
                Id = updatedUser.Id,
                Token = updatedUser.Token,
                RefreshToken = updatedUser.RefreshToken,
                Username = updatedUser.Username
            };

            return scResult;
        }

        public async Task<SocialCodeResult<AuthResponse>> Register(RegisterRequest registerRequest)
        {
            var scResult = new SocialCodeResult<AuthResponse>();

            if (!AuthRequestValidator.IsValidRegisterRequest(registerRequest))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Register request is not valid, all fields must be filled & passwords should match";
                return scResult;
            }

            //Convert & SaveUser
            var user = UserConverter.RegisterRequest_ToUser(registerRequest);


            //Generate user tokens
            var userTokens = GenerateNewTokens(user);

            //Validate tokens
            if (userTokens is null || userTokens.FirstOrDefault() is null || userTokens.LastOrDefault() is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                scResult.ErrorMsg = "Failed creating user tokens";
                return scResult;
            }

            //UpdateUser tokens
            user.Token = userTokens.FirstOrDefault();
            user.RefreshToken = userTokens.LastOrDefault();

            //Encrypt user password
            var encryptedPassword = PasswordUtils.HashPassword(user.Password);
            user.Password = encryptedPassword;

            //Save updated user
            var insertedUser = await _userRepository.Insert(user);
            if (insertedUser is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Existing username or email";
                return scResult;
            }

            scResult.Value = new AuthResponse
            {
                Id = insertedUser.Id,
                Token = insertedUser.Token,
                Username = insertedUser.Username,
                RefreshToken = insertedUser.RefreshToken
            };

            return scResult;
        }

        public async Task<SocialCodeResult<AuthResponse>> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var scResult = new SocialCodeResult<AuthResponse>();

            //Validate Token in refreshTokenRequest
            if (!IsValidToken(refreshTokenRequest))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Invalid request, token is not valid!";
                return scResult;
            }

            if (!CommonValidator.IsValidId(refreshTokenRequest.UserId))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Invalid user ID";
                return scResult;
            }

            var user = await _userRepository.GetUserById(refreshTokenRequest.UserId);
            if (user is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                scResult.ErrorMsg = "User not found";
                return scResult;
            }

            //Match Between tokenRequest token & user Token
            if (!refreshTokenRequest.RefreshToken.Equals(user.RefreshToken))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Refresh request Token doesn't match with user Token!";
                return scResult;
            }

            //Generate user tokens
            var userTokens = GenerateNewTokens(user);

            //Validate tokens
            if (userTokens is null || userTokens.FirstOrDefault() is null || userTokens.LastOrDefault() is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                scResult.ErrorMsg = "Failed creating user tokens";
                return scResult;
            }

            user.Token = userTokens.FirstOrDefault();
            user.RefreshToken = userTokens.LastOrDefault();

            var updatedUser = await _userRepository.ModifyUser(user.Id, user);
            if (updatedUser is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                scResult.ErrorMsg = "Failed updating user tokens";
                return scResult;
            }

            scResult.Value = new AuthResponse
            {
                Id = updatedUser.Id,
                Token = updatedUser.Token,
                RefreshToken = updatedUser.RefreshToken,
                Username = updatedUser.Username
            };

            return scResult;
        }

        private List<string> GenerateNewTokens(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key;
            try
            {
                key = Encoding.ASCII.GetBytes(_config.GetSection("JwtKey")?.ToString());
            }
            catch (Exception)
            {
                return null;
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = tokenHandler.WriteToken(token);
            var RefreshToken = RandomString(25) + Guid.NewGuid();

            return new List<string>
            {
                jwtToken,
                RefreshToken
            };
        }

        private static string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private bool IsValidToken(RefreshTokenRequest refreshTokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            byte[] key;
            try
            {
                key = Encoding.ASCII.GetBytes(_config.GetSection("JwtKey").ToString());
            }
            catch (Exception)
            {
                return false;
            }

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                RequireExpirationTime = false
            };

            try
            {
                jwtTokenHandler.ValidateToken(refreshTokenRequest.Token, tokenValidationParams, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase);

                    return result is true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}