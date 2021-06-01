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
using SocialCode.API.Requests.Users.Auth;
using SocialCode.API.Requests.Users.Register;
using SocialCode.API.Services.Users;
using SocialCode.API.Validators;
using SocialCode.API.Validators.Auth;
using SocialCode.Domain.User;

namespace SocialCode.API.Services.Auth
{
    public class AuthService :IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        
        public AuthService( IUserRepository userRepository, IConfiguration config)
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

            var encryptedPassword = PasswordUtils.HashPassword(loginRequest.Password);
            //Get user from Db
            var user = await _userRepository.GetByUsername(loginRequest.Username);
            
            if (user is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                scResult.ErrorMsg = "User not found!";
                return scResult;
            }
            
            if (!PasswordUtils.IsValidPassword(user.Password, loginRequest.Password))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                scResult.ErrorMsg = "The user password provided is not correct!";
                return scResult;
            }

            //Generate & Verify Tokens
            var tokens = GenerateNewTokens(user);
            if (tokens.FirstOrDefault() is null || tokens.LastOrDefault() is null)
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
                scResult.ErrorMsg = "Cant update user Tokens";
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
            }

            //Return dto with token & refresh token
            scResult.Value = new AuthResponse()
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
                scResult.ErrorMsg = "Check your username(contains @), email & password!";
                return scResult; 
            }
            
            //Convert & SaveUser
            var user = UserConverter.RegisterRequest_ToUser(registerRequest);
            var insertedUser = await _userRepository.Insert(user);
            
            if (insertedUser is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Existing user with same email or username, Email & Username are unique per user";
                return scResult;
            }
            //Generate user tokens
            var userTokens = GenerateNewTokens(insertedUser);
            
            //Validate tokens
            if (userTokens.FirstOrDefault() is null || userTokens.LastOrDefault() is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic; 
                scResult.ErrorMsg = "Failed creating user tokens";
                return scResult;
            }
            
            //UpdateUser tokens
            insertedUser.Token = userTokens.FirstOrDefault();
            insertedUser.RefreshToken = userTokens.LastOrDefault();

            //Encrypt user password
            var encryptedPassword = PasswordUtils.HashPassword(user.Password);
            user.Password = encryptedPassword;
            
            //Save updated user
            var updatedUser = await _userRepository.ModifyUser(insertedUser.Id, insertedUser);
            if (updatedUser is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.Generic;
                scResult.ErrorMsg = "Error saving user Tokens & EncryptedPassword";
                return scResult;
            }
            
            scResult.Value = new AuthResponse()
            {
                Id =updatedUser.Id,
                Token = updatedUser.Token,
                Username = updatedUser.Username,
                RefreshToken = updatedUser.RefreshToken
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
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                scResult.ErrorMsg = "User id doesn't match with any userID in DB";
                return scResult;
            }

            var user = await _userRepository.GetUserById(refreshTokenRequest.UserId);
            if (user is null)
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.NotFound;
                scResult.ErrorMsg = "Refresh request ID doesn't match with any user ID in database!";
                return scResult;
            }
            
            //Match Betwwen tokenRequest token & user Token
            if (!refreshTokenRequest.RefreshToken.Equals(user.RefreshToken))
            {
                scResult.ErrorTypes = SocialCodeErrorTypes.BadRequest;
                scResult.ErrorMsg = "Refresh request Token doesn't match with user Token!";
                return scResult;
            }

            //Generate user tokens
            var userTokens = GenerateNewTokens(user);
            
            //Validate tokens
            if (userTokens.FirstOrDefault() is null || userTokens.LastOrDefault() is null)
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
            var key = Encoding.ASCII.GetBytes(_config.GetSection("JwtKey").ToString());
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new []
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            var jwtToken = tokenHandler.WriteToken(token);
            var RefreshToken = RandomString(25) + Guid.NewGuid();
            
            return new List<string>()
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
            var key = Encoding.ASCII.GetBytes(_config.GetSection("JwtKey").ToString());
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
