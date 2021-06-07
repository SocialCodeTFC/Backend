using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Users.Auth;
using SocialCode.API.Services.Auth;
using SocialCode.Domain.User;
using SocialCode.UnitTesting.Resources;
using Xunit;

namespace SocialCode.UnitTesting
{
    public class AuthServiceTest:IClassFixture<TestFixture>
    {
        private readonly IConfiguration _config;
        public AuthServiceTest(TestFixture fixture)
        {
            _config = fixture.Configuration ;
        }
        
        [Fact]
        public async Task LogIn_ValidLoginRequest_ShouldReturn_SocialCodeResult()
        {

            var loginRequest = new LoginRequest
            {
                Username = "@Username",
                Password = "Password"
            };
            
            var expectedResult = new SocialCodeResult<AuthResponse>()
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

            userRepositoryMock.Setup(repo => repo.GetByUsername(loginRequest.Username)).ReturnsAsync(new User()
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
            
            userRepositoryMock.Setup(repo => repo.ModifyUser(UnitTestConstants.VALID_ID, It.IsAny<User>())).ReturnsAsync(new User()
            {
                Id = UnitTestConstants.VALID_ID,
                Email = "test@socialcode.com",
                Token = UnitTestConstants.VALID_TOKEN,
                Password = "$MYHASH$V1$10000$Mk/UlLbYfB9LkY12gZEF+xkOKahy3fnt5K98px/nZtX8qQES",
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
            
            userRepositoryMock.Verify(repo => repo.GetByUsername(loginRequest.Username), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser(UnitTestConstants.VALID_ID, It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task LogIn_InvalidLoginRequest_ShouldReturn_SocialCodeResult()
        {
            var loginRequest = new LoginRequest
            {
                Username = "Username",
                Password = "Password"
            };
            
            var expectedResult = new SocialCodeResult<AuthResponse>()
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
            userRepositoryMock.Verify(repo => repo.GetByUsername(loginRequest.Username), Times.Never);
            userRepositoryMock.Verify(repo => repo.ModifyUser( It.IsAny<string>(),It.IsAny<User>()), Times.Never);
        }
        
        [Fact]
        public async Task LogIn_UserNotFound_ShouldReturn_SocialCodeNotFoundErr()
        {
            var loginRequest = new LoginRequest
            {
                Username = "@Username",
                Password = "Password"
            };
            
            var expectedResult = new SocialCodeResult<AuthResponse>()
            {
                Value = null,
                ErrorMsg = "User not found!",
                ErrorTypes = SocialCodeErrorTypes.NotFound
            };

            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(repo => repo.GetByUsername(loginRequest.Username)).ReturnsAsync((User) null);

            var authService = new AuthService(userRepositoryMock.Object, _config);

            var serviceResult = await authService.LogIn(loginRequest);
            
            Assert.False(serviceResult.IsValid());
            Assert.Null(serviceResult.Value);
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Equal(expectedResult.ErrorTypes, serviceResult.ErrorTypes);
            userRepositoryMock.Verify(repo => repo.GetByUsername(loginRequest.Username), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser( It.IsAny<string>(),It.IsAny<User>()), Times.Never);
        }
        
        [Fact]
        public async Task LogIn_PasswordsDoNotMatch_ShouldReturn_SocialCodeBadRequestErr()
        {
            var loginRequest = new LoginRequest
            {
                Username = "@Username",
                Password = "Password1"
            };
            
            var expectedResult = new SocialCodeResult<AuthResponse>()
            {
                Value = null,
                ErrorMsg = "Passwords do not match",
                ErrorTypes = SocialCodeErrorTypes.BadRequest
            };

            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(repo => repo.GetByUsername(loginRequest.Username)).ReturnsAsync(new User
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
            userRepositoryMock.Verify(repo => repo.GetByUsername(loginRequest.Username), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser( It.IsAny<string>(),It.IsAny<User>()), Times.Never);
        }
        
        [Fact]
        public async Task LogIn_ConfigThrowsAnyException_ShouldReturn_SocialCodeBadRequestErr()
        {
            var loginRequest = new LoginRequest
            {
                Username = "@Username",
                Password = "Password"
            };
            
            var expectedResult = new SocialCodeResult<AuthResponse>()
            {
                Value = null,
                ErrorMsg = "Failed creating user tokens",
                ErrorTypes = SocialCodeErrorTypes.Generic
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var configMock = new Mock<IConfiguration>();

            userRepositoryMock.Setup(repo => repo.GetByUsername(loginRequest.Username)).ReturnsAsync(new User
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
            userRepositoryMock.Verify(repo => repo.GetByUsername(loginRequest.Username), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser( It.IsAny<string>(),It.IsAny<User>()), Times.Never);
            configMock.Verify(config => config.GetSection("JwtKey").ToString(), Times.Once);
        }
        
    }
}