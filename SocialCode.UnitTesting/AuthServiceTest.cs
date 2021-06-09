using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Users.Auth;
using SocialCode.API.Requests.Users.Register;
using SocialCode.API.Services.Auth;
using SocialCode.Domain.User;
using SocialCode.UnitTesting.Resources;
using Xunit;
using Xunit.Abstractions;

namespace SocialCode.UnitTesting
{
    public class AuthServiceTest : IClassFixture<TestFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IConfiguration _config;

        public AuthServiceTest(TestFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _config = fixture.Configuration;
        }

        [Fact]
        public async Task LogIn_SuccessfulLogin_ShouldReturn_SocialCodeResult()
        {
            var loginRequest = new LoginRequest
            {
                Username = "@Username",
                Password = "Password"
            };

            var expectedResult = new SocialCodeResult<AuthResponse>
            {
                Value = new AuthResponse
                {
                    Username = loginRequest.Username,
                    Id = UnitTestConstants.VALID_ID
                },
                ErrorMsg = null,
                ErrorTypes = SocialCodeErrorTypes.Generic
            };

            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserByUsername(loginRequest.Username)).ReturnsAsync(new User
            {
                Id = UnitTestConstants.VALID_ID,
                Email = "test@socialcode.com",
                Token = "Token",
                Password = UnitTestConstants.ENCRYPTED_VALID_PASSWORD,
                Username = "@Username",
                FirstName = "Test",
                LastName = "Dummy",
                RefreshToken = UnitTestConstants.VALID_REFRESH_TOKEN
            });

            userRepositoryMock.Setup(repo => repo.ModifyUser(UnitTestConstants.VALID_ID, It.IsAny<User>()))
                .ReturnsAsync(new User
                {
                    Id = UnitTestConstants.VALID_ID,
                    Email = "test@socialcode.com",
                    Token = UnitTestConstants.VALID_TOKEN,
                    Password = UnitTestConstants.ENCRYPTED_VALID_PASSWORD,
                    Username = "@Username",
                    FirstName = "Test",
                    LastName = "Dummy",
                    RefreshToken = UnitTestConstants.VALID_REFRESH_TOKEN
                });

            var authService = new AuthService(userRepositoryMock.Object, _config);

            var serviceResult = await authService.LogIn(loginRequest);

            Assert.True(serviceResult.IsValid());
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            Assert.Equal(expectedResult.Value.Username, serviceResult.Value.Username);
            Assert.Equal(expectedResult.Value.Id, serviceResult.Value.Id);
            Assert.NotEqual("Token", serviceResult.Value.Token);
            Assert.NotEqual("Refresh", serviceResult.Value.RefreshToken);

            userRepositoryMock.Verify(repo => repo.GetUserByUsername(loginRequest.Username), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser(UnitTestConstants.VALID_ID, It.IsAny<User>()),
                Times.Once);
        }
        [Fact]
        public async Task LogIn_InvalidLoginRequest_ShouldReturn_SocialCodeResult()
        {
            var loginRequest = new LoginRequest
            {
                Username = "Username",
                Password = "Password"
            };

            var expectedResult = new SocialCodeResult<AuthResponse>
            {
                Value = null,
                ErrorMsg = "Login request is not valid",
                ErrorTypes = SocialCodeErrorTypes.BadRequest
            };

            var userRepositoryMock = new Mock<IUserRepository>();

            var authService = new AuthService(userRepositoryMock.Object, _config);

            var serviceResult = await authService.LogIn(loginRequest);

            Assert.False(serviceResult.IsValid());
            Assert.Null(serviceResult.Value);
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            userRepositoryMock.Verify(repo => repo.GetUserByUsername(loginRequest.Username), Times.Never);
            userRepositoryMock.Verify(repo => repo.ModifyUser(It.IsAny<string>(), It.IsAny<User>()), Times.Never);
        }
        [Fact]
        public async Task LogIn_UserNotFound_ShouldReturn_SocialCodeNotFoundErr()
        {
            var loginRequest = new LoginRequest
            {
                Username = "@Username",
                Password = "Password"
            };

            var expectedResult = new SocialCodeResult<AuthResponse>
            {
                Value = null,
                ErrorMsg = "User not found!",
                ErrorTypes = SocialCodeErrorTypes.NotFound
            };

            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserByUsername(loginRequest.Username)).ReturnsAsync((User) null);

            var authService = new AuthService(userRepositoryMock.Object, _config);

            var serviceResult = await authService.LogIn(loginRequest);

            Assert.False(serviceResult.IsValid());
            Assert.Null(serviceResult.Value);
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            userRepositoryMock.Verify(repo => repo.GetUserByUsername(loginRequest.Username), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser(It.IsAny<string>(), It.IsAny<User>()), Times.Never);
        }
        [Fact]
        public async Task LogIn_PasswordsDoNotMatch_ShouldReturn_SocialCodeBadRequestErr()
        {
            var loginRequest = new LoginRequest
            {
                Username = "@Username",
                Password = "Password1"
            };

            var expectedResult = new SocialCodeResult<AuthResponse>
            {
                Value = null,
                ErrorMsg = "Passwords do not match",
                ErrorTypes = SocialCodeErrorTypes.BadRequest
            };

            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserByUsername(loginRequest.Username)).ReturnsAsync(new User
            {
                Id = UnitTestConstants.VALID_ID,
                Email = "test@socialcode.com",
                Token = "Token",
                Password = "$MYHASH$V1$10000$Mk/UlLbYfB9LkY12gZEF+xkOKahy3fnt5K98px/nZtX8qQES",
                Username = "@Username",
                FirstName = "Test",
                LastName = "Dummy",
                RefreshToken = "Refresh"
            });

            var authService = new AuthService(userRepositoryMock.Object, _config);

            var serviceResult = await authService.LogIn(loginRequest);

            Assert.False(serviceResult.IsValid());
            Assert.Null(serviceResult.Value);
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            userRepositoryMock.Verify(repo => repo.GetUserByUsername(loginRequest.Username), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser(It.IsAny<string>(), It.IsAny<User>()), Times.Never);
        }
        [Fact]
        public async Task LogIn_FailCreatingTokens_ShouldReturn_SocialCodeGenericErr()
        {
            var loginRequest = new LoginRequest
            {
                Username = "@Username",
                Password = "Password"
            };

            var expectedResult = new SocialCodeResult<AuthResponse>
            {
                Value = null,
                ErrorMsg = "Failed creating user tokens",
                ErrorTypes = SocialCodeErrorTypes.Generic
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var configMock = new Mock<IConfiguration>();

            userRepositoryMock.Setup(repo => repo.GetUserByUsername(loginRequest.Username)).ReturnsAsync(new User
            {
                Id = UnitTestConstants.VALID_ID,
                Email = "test@socialcode.com",
                Token = "Token",
                Password = "$MYHASH$V1$10000$Mk/UlLbYfB9LkY12gZEF+xkOKahy3fnt5K98px/nZtX8qQES",
                Username = "@Username",
                FirstName = "Test",
                LastName = "Dummy",
                RefreshToken = "Refresh"
            });

            configMock.Setup(config => config.GetSection("JwtKey").ToString()).Throws(new Exception());

            var authService = new AuthService(userRepositoryMock.Object, configMock.Object);
            var serviceResult = await authService.LogIn(loginRequest);

            Assert.False(serviceResult.IsValid());
            Assert.Null(serviceResult.Value);
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            userRepositoryMock.Verify(repo => repo.GetUserByUsername(loginRequest.Username), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser(It.IsAny<string>(), It.IsAny<User>()), Times.Never);
            configMock.Verify(config => config.GetSection("JwtKey").ToString(), Times.Once);
        }
        [Fact]
        public async Task LogIn_RefreshingUserTokensFail_ShouldReturn_SocialCodeGenericErr()
        {
            var loginRequest = new LoginRequest
            {
                Username = "@Username",
                Password = "Password"
            };

            var expectedResult = new SocialCodeResult<AuthResponse>
            {
                Value = null,
                ErrorMsg = "Failed to update user Tokens",
                ErrorTypes = SocialCodeErrorTypes.Generic
            };

            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserByUsername(loginRequest.Username)).ReturnsAsync(new User
            {
                Id = UnitTestConstants.VALID_ID,
                Email = "test@socialcode.com",
                Token = "Token",
                Password = "$MYHASH$V1$10000$Mk/UlLbYfB9LkY12gZEF+xkOKahy3fnt5K98px/nZtX8qQES",
                Username = "@Username",
                FirstName = "Test",
                LastName = "Dummy",
                RefreshToken = "Refresh"
            });

            userRepositoryMock.Setup(repo => repo.ModifyUser(UnitTestConstants.VALID_ID, It.IsAny<User>()))
                .ReturnsAsync((User) null);

            var authService = new AuthService(userRepositoryMock.Object, _config);
            var serviceResult = await authService.LogIn(loginRequest);

            Assert.False(serviceResult.IsValid());
            Assert.Null(serviceResult.Value);
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            userRepositoryMock.Verify(repo => repo.GetUserByUsername(loginRequest.Username), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser(It.IsAny<string>(), It.IsAny<User>()), Times.Once);
        }
        
        
        [Fact]
        public async Task Register_SuccessfulRegister_ShouldReturn_SocialCodeAuthResponse()
        {
            var registerRequest = new RegisterRequest
            {
                Email = "test@socialcode666.com",
                Password = "Password",
                RepeatPassword = "Password",
                Username = "@Usernametest",
                FirstName = "User",
                LastName = "Test"
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.Insert(It.IsAny<User>())).ReturnsAsync(new User
            {
                Id = UnitTestConstants.VALID_ID,
                Email = "test@socialcode666.com",
                Password = "Password",
                Username = "@Usernametest",
                FirstName = "User",
                LastName = "Test",
                Token = UnitTestConstants.VALID_TOKEN,
                RefreshToken = UnitTestConstants.VALID_REFRESH_TOKEN,
                SavedPostsIds = null
            });

            var authService = new AuthService(userRepositoryMock.Object, _config);
            var serviceResult = await authService.Register(registerRequest);

            Assert.True(serviceResult.IsValid());
            Assert.Null(serviceResult.ErrorMsg);
            Assert.Equal(UnitTestConstants.VALID_ID, serviceResult.Value.Id);
            Assert.Equal(UnitTestConstants.VALID_TOKEN, serviceResult.Value.Token);
            Assert.Equal(UnitTestConstants.VALID_REFRESH_TOKEN, serviceResult.Value.RefreshToken);
            Assert.Equal(registerRequest.Username, serviceResult.Value.Username);
            userRepositoryMock.Verify(repo => repo.Insert(It.IsAny<User>()), Times.Once);
        }
        [Fact]
        public async Task Register_InvalidRegisterRequest_ShouldReturn_SocialCodeBadRequestErr()
        {
            var registerRequest = new RegisterRequest
            {
                Email = "test@socialcode666.com",
                Password = "Password",
                RepeatPassword = "Password",
                Username = "Usernametest",
                FirstName = "User",
                LastName = "Test"
            };

            var expectedResult = new SocialCodeResult<AuthResponse>
            {
                Value = null,
                ErrorMsg = "Register request is not valid, all fields must be filled & passwords should match",
                ErrorTypes = SocialCodeErrorTypes.BadRequest
            };

            var userRepositoryMock = new Mock<IUserRepository>();

            var authService = new AuthService(userRepositoryMock.Object, _config);


            var serviceResult = await authService.Register(registerRequest);

            Assert.False(serviceResult.IsValid());
            Assert.Null(serviceResult.Value);
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            userRepositoryMock.Verify(repo => repo.Insert(It.IsAny<User>()), Times.Never);
        }
        [Fact]
        public async Task Register_FailCreatingTokens_ShouldReturn_SocialCodeBadRequestErr()
        {
            var registerRequest = new RegisterRequest
            {
                Email = "test@socialcode666.com",
                Password = "Password",
                RepeatPassword = "Password",
                Username = "@Usernametest",
                FirstName = "User",
                LastName = "Test"
            };

            var expectedResult = new SocialCodeResult<AuthResponse>
            {
                Value = null,
                ErrorMsg = "Failed creating user tokens",
                ErrorTypes = SocialCodeErrorTypes.Generic
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(config => config.GetSection("JwtKey").ToString()).Throws(new Exception());

            var authService = new AuthService(userRepositoryMock.Object, configMock.Object);
            var serviceResult = await authService.Register(registerRequest);

            Assert.False(serviceResult.IsValid());
            Assert.Null(serviceResult.Value);
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            userRepositoryMock.Verify(repo => repo.Insert(It.IsAny<User>()), Times.Never);
            configMock.Verify(config => config.GetSection("JwtKey").ToString(), Times.Once);
        }
        [Fact]
        public async Task Register_RepositoryReturnsNull_ShouldReturn_SocialCodeBadRequestErr()
        {
            var registerRequest = new RegisterRequest
            {
                Email = "test@socialcode666.com",
                Password = "Password",
                RepeatPassword = "Password",
                Username = "@Usernametest",
                FirstName = "User",
                LastName = "Test"
            };

            var expectedResult = new SocialCodeResult<AuthResponse>
            {
                Value = null,
                ErrorMsg = "Existing username or email",
                ErrorTypes = SocialCodeErrorTypes.BadRequest
            };

            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(repo => repo.Insert(It.IsAny<User>())).ReturnsAsync((User) null);
            
            var authService = new AuthService(userRepositoryMock.Object, _config);
            var serviceResult = await authService.Register(registerRequest);

            Assert.False(serviceResult.IsValid());
            Assert.Null(serviceResult.Value);
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            userRepositoryMock.Verify(repo => repo.Insert(It.IsAny<User>()), Times.Once);
        }

        
        [Fact]
        public async Task Refresh_SuccessfulRefresh_ShouldReturn_SocialCodeAuthResponse()
        {
            var refreshTokenRequest = new RefreshTokenRequest()
            {
                Token = UnitTestConstants.VALID_TOKEN,
                RefreshToken = UnitTestConstants.VALID_REFRESH_TOKEN,
                UserId = UnitTestConstants.VALID_ID
            };

            var userRepositoryMock = new Mock<IUserRepository>();

            var obtainedUser = new User
            {
                Id = UnitTestConstants.VALID_ID,
                Email = "test@socialcode666.com",
                Password = UnitTestConstants.ENCRYPTED_VALID_PASSWORD,
                Username = "@Usernametest",
                FirstName = "User",
                LastName = "Test",
                Token = UnitTestConstants.VALID_TOKEN,
                RefreshToken = UnitTestConstants.VALID_REFRESH_TOKEN,
                SavedPostsIds = null
            };

            var modifiedUser = new User
            {
                Id = UnitTestConstants.VALID_ID,
                Email = "test@socialcode666.com",
                Password = UnitTestConstants.ENCRYPTED_VALID_PASSWORD,
                Username = "@Usernametest",
                FirstName = "User",
                LastName = "Test",
                Token = UnitTestConstants.NEW_VALID_TOKEN,
                RefreshToken = UnitTestConstants.NEW_VALID_REFRESH_TOKEN,
                SavedPostsIds = obtainedUser.SavedPostsIds
            };
            
            userRepositoryMock.Setup(repo => repo.GetUserById(refreshTokenRequest.UserId)).ReturnsAsync(obtainedUser);

            userRepositoryMock.Setup(repo => repo.ModifyUser(UnitTestConstants.VALID_ID, It.IsAny<User>()))
                .ReturnsAsync(modifiedUser);

            var authService = new AuthService(userRepositoryMock.Object, _config);
            var serviceResult = await authService.RefreshToken(refreshTokenRequest);
            
            Assert.True(serviceResult.IsValid());
            Assert.Null(serviceResult.ErrorMsg);
            Assert.Equal(obtainedUser.Id, serviceResult.Value.Id);
            Assert.Equal( UnitTestConstants.NEW_VALID_TOKEN, serviceResult.Value.Token);
            Assert.Equal( obtainedUser.Username, serviceResult.Value.Username);
            Assert.Equal(UnitTestConstants.NEW_VALID_REFRESH_TOKEN, serviceResult.Value.RefreshToken);
            userRepositoryMock.Verify(repo => repo.GetUserById(refreshTokenRequest.UserId), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser(refreshTokenRequest.UserId, It.IsAny<User>()), Times.Once);
        }
        [Fact]
        public async Task Refresh_InvalidToken_ShouldReturn_SocialCodeBadRequestErr()
        {
            var refreshTokenRequest = new RefreshTokenRequest()
            {
                Token = "InvalidToken",
                RefreshToken = UnitTestConstants.VALID_REFRESH_TOKEN,
                UserId = UnitTestConstants.VALID_ID
            };
            
            var expectedResult = new SocialCodeResult<AuthResponse>()
            {
                Value = null,
                ErrorMsg = "Invalid request, token is not valid!",
                ErrorTypes = SocialCodeErrorTypes.BadRequest
            };
            
            var userRepositoryMock = new Mock<IUserRepository>();

            var authService = new AuthService(userRepositoryMock.Object, _config);

            var serviceResult = await authService.RefreshToken(refreshTokenRequest);
            
            Assert.False(serviceResult.IsValid());
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            Assert.Null(serviceResult.Value);
            userRepositoryMock.Verify(repo => repo.GetUserById(It.IsAny<string>()), Times.Never);
            userRepositoryMock.Verify(repo => repo.ModifyUser(It.IsAny<string>(), It.IsAny<User>()), Times.Never);
        }
        [Fact]
        public async Task Refresh_InvalidID_ShouldReturn_SocialCodeBadRequestErr()
        {
            var refreshTokenRequest = new RefreshTokenRequest()
            {
                Token = UnitTestConstants.VALID_TOKEN,
                RefreshToken = UnitTestConstants.VALID_REFRESH_TOKEN,
                UserId = UnitTestConstants.INVALID_ID
            };
            
            var expectedResult = new SocialCodeResult<AuthResponse>()
            {
                Value = null,
                ErrorMsg = "Invalid user ID",
                ErrorTypes = SocialCodeErrorTypes.BadRequest
            };
            
            var userRepositoryMock = new Mock<IUserRepository>();

            var authService = new AuthService(userRepositoryMock.Object, _config);

            var serviceResult = await authService.RefreshToken(refreshTokenRequest);
            
            Assert.False(serviceResult.IsValid());
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            Assert.Null(serviceResult.Value);
            userRepositoryMock.Verify(repo => repo.GetUserById(It.IsAny<string>()), Times.Never);
            userRepositoryMock.Verify(repo => repo.ModifyUser(It.IsAny<string>(), It.IsAny<User>()), Times.Never);
        }
        [Fact]
        public async Task Refresh_UserNotFound_ShouldReturn_SocialCodeNotFoundErr()
        {
            var refreshTokenRequest = new RefreshTokenRequest()
            {
                Token = UnitTestConstants.VALID_TOKEN,
                RefreshToken = UnitTestConstants.VALID_REFRESH_TOKEN,
                UserId = UnitTestConstants.VALID_ID
            };
            
            var expectedResult = new SocialCodeResult<AuthResponse>()
            {
                Value = null,
                ErrorMsg = "User not found",
                ErrorTypes = SocialCodeErrorTypes.NotFound
            };
            
            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserById(UnitTestConstants.VALID_ID)).ReturnsAsync((User) null);
            
            var authService = new AuthService(userRepositoryMock.Object, _config);

            var serviceResult = await authService.RefreshToken(refreshTokenRequest);
            
            Assert.False(serviceResult.IsValid());
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            Assert.Null(serviceResult.Value);
            userRepositoryMock.Verify(repo => repo.GetUserById(It.IsAny<string>()), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser(It.IsAny<string>(), It.IsAny<User>()), Times.Never);
        }
        [Fact]
        public async Task Refresh_TokensDoNotMatch_ShouldReturn_SocialCodeBadRequest()
        {
            var refreshTokenRequest = new RefreshTokenRequest()
            {
                Token = UnitTestConstants.VALID_TOKEN,
                RefreshToken = UnitTestConstants.VALID_REFRESH_TOKEN,
                UserId = UnitTestConstants.VALID_ID
            };
            
            var expectedResult = new SocialCodeResult<AuthResponse>()
            {
                Value = null,
                ErrorMsg = "Refresh request Token doesn't match with user Token!",
                ErrorTypes = SocialCodeErrorTypes.BadRequest
            };
            
            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserById(UnitTestConstants.VALID_ID)).ReturnsAsync(new User
            {
                Email = "test@user.com",
                Id = UnitTestConstants.VALID_ID,
                Password = UnitTestConstants.ENCRYPTED_VALID_PASSWORD,
                Token = UnitTestConstants.VALID_TOKEN,
                RefreshToken = UnitTestConstants.NEW_VALID_REFRESH_TOKEN,
                Username = "@Username",
                FirstName = "User",
                LastName = "Test",
                SavedPostsIds = null
            });
            
            var authService = new AuthService(userRepositoryMock.Object, _config);

            var serviceResult = await authService.RefreshToken(refreshTokenRequest);
            
            Assert.False(serviceResult.IsValid());
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            Assert.Null(serviceResult.Value);
            userRepositoryMock.Verify(repo => repo.GetUserById(UnitTestConstants.VALID_ID), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser(It.IsAny<string>(), It.IsAny<User>()), Times.Never);
        }
        [Fact]
        public async Task Refresh_FailedToGenerateTokens_ShouldReturn_SocialCodeGenericErr()
        {
            var refreshTokenRequest = new RefreshTokenRequest()
            {
                Token = UnitTestConstants.VALID_TOKEN,
                RefreshToken = UnitTestConstants.VALID_REFRESH_TOKEN,
                UserId = UnitTestConstants.VALID_ID
            };
            
            var expectedResult = new SocialCodeResult<AuthResponse>()
            {
                Value = null,
                ErrorMsg = "Failed creating user tokens",
                ErrorTypes = SocialCodeErrorTypes.Generic
            };
            
            var userRepositoryMock = new Mock<IUserRepository>();
            var configMock = new Mock<IConfiguration>();

            userRepositoryMock.Setup(repo => repo.GetUserById(UnitTestConstants.VALID_ID)).ReturnsAsync(new User
            {
                Email = "test@user.com",
                Id = UnitTestConstants.VALID_ID,
                Password = UnitTestConstants.ENCRYPTED_VALID_PASSWORD,
                Token = UnitTestConstants.VALID_TOKEN,
                RefreshToken = UnitTestConstants.VALID_REFRESH_TOKEN,
                Username = "@Username",
                FirstName = "User",
                LastName = "Test",
                SavedPostsIds = null
            });

            configMock.SetupSequence(config => config.GetSection("JwtKey").ToString()).Returns(_config.GetSection("JwtKey").ToString)
                .Throws(new Exception());
            
            var authService = new AuthService(userRepositoryMock.Object, configMock.Object);

            var serviceResult = await authService.RefreshToken(refreshTokenRequest);
            
            Assert.False(serviceResult.IsValid());
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            Assert.Null(serviceResult.Value);
            userRepositoryMock.Verify(repo => repo.GetUserById(UnitTestConstants.VALID_ID), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser(It.IsAny<string>(), It.IsAny<User>()), Times.Never);
            configMock.Verify(config => config.GetSection("JwtKey").ToString(), Times.Exactly(2));
        }
        [Fact]
        public async Task Refresh_FailedToUpdateUserTokens_ShouldReturn_SocialCodeGenericErr()
        {
            var refreshTokenRequest = new RefreshTokenRequest()
            {
                Token = UnitTestConstants.VALID_TOKEN,
                RefreshToken = UnitTestConstants.VALID_REFRESH_TOKEN,
                UserId = UnitTestConstants.VALID_ID
            };
            
            var expectedResult = new SocialCodeResult<AuthResponse>()
            {
                Value = null,
                ErrorMsg = "Failed updating user tokens",
                ErrorTypes = SocialCodeErrorTypes.Generic
            };
            
            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserById(UnitTestConstants.VALID_ID)).ReturnsAsync(new User
            {
                Email = "test@user.com",
                Id = UnitTestConstants.VALID_ID,
                Password = UnitTestConstants.ENCRYPTED_VALID_PASSWORD,
                Token = UnitTestConstants.VALID_TOKEN,
                RefreshToken = UnitTestConstants.VALID_REFRESH_TOKEN,
                Username = "@Username",
                FirstName = "User",
                LastName = "Test",
                SavedPostsIds = null
            });

            userRepositoryMock.Setup(repo => repo.ModifyUser(UnitTestConstants.VALID_ID, It.IsAny<User>()))
                .ReturnsAsync((User)null);
            
            var authService = new AuthService(userRepositoryMock.Object, _config);

            var serviceResult = await authService.RefreshToken(refreshTokenRequest);
            
            Assert.False(serviceResult.IsValid());
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            Assert.Null(serviceResult.Value);
            userRepositoryMock.Verify(repo => repo.GetUserById(UnitTestConstants.VALID_ID), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser(It.IsAny<string>(), It.IsAny<User>()), Times.Once);
        }
    }
}