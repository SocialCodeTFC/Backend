using System.Threading.Tasks;
using Moq;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Users;
using SocialCode.API.Services.Users;
using SocialCode.Domain.User;
using SocialCode.UnitTesting.Resources;
using Xunit;

namespace SocialCode.UnitTesting
{
    public class UserServiceTest
    {
        
        [Fact]
        public async Task GetById_ValidID_ShouldReturn_SocialCodeResult()
        {
            var expectedResult = new SocialCodeResult<UserDataResponse>
            {
                Value = new UserDataResponse
                {
                    Email = "test@test.com",
                    Id = UnitTestConstants.VALID_ID,
                    FirstName = "Test",
                    LastName = "Dummy",
                    Username = "@Test"
                },
                ErrorMsg = null,
                ErrorTypes = SocialCodeErrorTypes.Generic
            };

            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserById(UnitTestConstants.VALID_ID)).ReturnsAsync(new User
            {
                Id = UnitTestConstants.VALID_ID,
                Username = expectedResult.Value.Username,
                Email = expectedResult.Value.Email,
                Token = "Token",
                Password = "Password",
                FirstName = expectedResult.Value.FirstName,
                LastName = expectedResult.Value.LastName
            });

            var userService = new UserService(userRepositoryMock.Object);
            var serviceResult = await userService.GetUserById(UnitTestConstants.VALID_ID);

            Assert.True(serviceResult.IsValid());
            Assert.Null(serviceResult.ErrorMsg);
            Assert.True(serviceResult.ErrorTypes is SocialCodeErrorTypes.Generic);
            Assert.Equal(expectedResult.Value.Id, serviceResult.Value.Id);
            Assert.Equal(expectedResult.Value.Email, serviceResult.Value.Email);
            Assert.Equal(expectedResult.Value.FirstName, serviceResult.Value.FirstName);
            Assert.Equal(expectedResult.Value.LastName, serviceResult.Value.LastName);
            Assert.Equal(expectedResult.Value.Username, serviceResult.Value.Username);
            userRepositoryMock.Verify(repo => repo.GetUserById(UnitTestConstants.VALID_ID), Times.Once);
        }

        [Fact]
        public async Task GetById_NotValidId_ShouldReturn_SocialCodeBadRequestErr()
        {
            var expectedResult = new SocialCodeResult<UserDataResponse>
            {
                Value = null,
                ErrorMsg = "ID is not valid",
                ErrorTypes = SocialCodeErrorTypes.BadRequest
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var userService = new UserService(userRepositoryMock.Object);

            var serviceResult = await userService.GetUserById(UnitTestConstants.INVALID_ID);

            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.True(expectedResult.ErrorTypes is SocialCodeErrorTypes.BadRequest);
            Assert.Null(expectedResult.Value);
            Assert.False(serviceResult.IsValid());
            userRepositoryMock.Verify(repo => repo.GetUserById(UnitTestConstants.INVALID_ID), Times.Never);
        }

        [Fact]
        public async Task GetById_UserNotFound_ShouldReturn_SocialCodeNotFoundErr()
        {
            var expectedResult = new SocialCodeResult<UserDataResponse>
            {
                Value = null,
                ErrorMsg = "User not found",
                ErrorTypes = SocialCodeErrorTypes.NotFound
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.GetUserById(UnitTestConstants.VALID_ID)).ReturnsAsync((User) null);
            var userService = new UserService(userRepositoryMock.Object);

            var serviceResult = await userService.GetUserById(UnitTestConstants.VALID_ID);
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.True(expectedResult.ErrorTypes is SocialCodeErrorTypes.NotFound);
            Assert.Null(expectedResult.Value);
            Assert.False(serviceResult.IsValid());
            userRepositoryMock.Verify(repo => repo.GetUserById(UnitTestConstants.VALID_ID), Times.Once);
        }

        [Fact]
        public async Task Delete_ValidID_ShouldReturn_SocialCodeResult()
        {
            var expectedResult = new SocialCodeResult<UserDataResponse>
            {
                Value = new UserDataResponse
                {
                    Email = "test@test.com",
                    Id = UnitTestConstants.VALID_ID,
                    FirstName = "Test",
                    LastName = "Dummy",
                    Username = "@Test"
                },
                ErrorMsg = null,
                ErrorTypes = SocialCodeErrorTypes.Generic
            };

            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(repo => repo.DeleteUser(UnitTestConstants.VALID_ID)).ReturnsAsync(new User
            {
                Email = expectedResult.Value.Email,
                Password = "Password",
                Token = "Token",
                Id = UnitTestConstants.VALID_ID,
                Username = expectedResult.Value.Username,
                FirstName = expectedResult.Value.FirstName,
                LastName = expectedResult.Value.LastName
            });

            var userService = new UserService(userRepositoryMock.Object);

            var serviceResult = await userService.DeleteUser(UnitTestConstants.VALID_ID);


            Assert.True(serviceResult.IsValid());
            Assert.Null(serviceResult.ErrorMsg);
            Assert.True(expectedResult.ErrorTypes is SocialCodeErrorTypes.Generic);
            Assert.Equal(expectedResult.Value.Id, serviceResult.Value.Id);
            Assert.Equal(expectedResult.Value.Email, serviceResult.Value.Email);
            Assert.Equal(expectedResult.Value.FirstName, serviceResult.Value.FirstName);
            Assert.Equal(expectedResult.Value.LastName, serviceResult.Value.LastName);
            Assert.Equal(expectedResult.Value.Username, serviceResult.Value.Username);
            userRepositoryMock.Verify(repo => repo.DeleteUser(UnitTestConstants.VALID_ID), Times.Once);
        }

        [Fact]
        public async Task Delete_InvalidId_ShouldReturn_SocialCodeBadRequestErr()
        {
            var expectedResult = new SocialCodeResult<UserDataResponse>
            {
                Value = null,
                ErrorMsg = "ID is not valid",
                ErrorTypes = SocialCodeErrorTypes.BadRequest
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var userService = new UserService(userRepositoryMock.Object);

            var serviceResult = await userService.DeleteUser(UnitTestConstants.INVALID_ID);

            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.True(expectedResult.ErrorTypes is SocialCodeErrorTypes.BadRequest);
            Assert.Null(expectedResult.Value);
            Assert.False(serviceResult.IsValid());
            userRepositoryMock.Verify(repo => repo.DeleteUser(UnitTestConstants.INVALID_ID), Times.Never);
        }

        [Fact]
        public async Task Delete_UserNotFound_ShouldReturn_SocialCodeNotFoundErr()
        {
            var expectedResult = new SocialCodeResult<UserDataResponse>
            {
                Value = null,
                ErrorMsg = "User not found",
                ErrorTypes = SocialCodeErrorTypes.NotFound
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.DeleteUser(UnitTestConstants.VALID_ID)).ReturnsAsync((User) null);
            var userService = new UserService(userRepositoryMock.Object);

            var serviceResult = await userService.DeleteUser(UnitTestConstants.VALID_ID);
            Assert.Equal(expectedResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.True(expectedResult.ErrorTypes is SocialCodeErrorTypes.NotFound);
            Assert.Null(expectedResult.Value);
            Assert.False(serviceResult.IsValid());
            userRepositoryMock.Verify(repo => repo.DeleteUser(UnitTestConstants.VALID_ID), Times.Once);
        }

        [Fact]
        public async Task Modify_ValidUpdatedData_ShouldReturn_SocialCodeResult()
        {
            var updatedUserDataRequest = new UserDataRequest
            {
                Email = "test@test.com",
                Id = UnitTestConstants.VALID_ID,
                FirstName = "Test",
                LastName = "Dummy",
                Username = "@Test"
            };

            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserById(UnitTestConstants.VALID_ID)).ReturnsAsync(new User
            {
                Id = UnitTestConstants.VALID_ID,
                Email = "user@email.com",
                Password = "Password",
                Username = "@Username",
                FirstName = "FirstName",
                LastName = "LastName",
                Token = "Token",
                RefreshToken = "Refresh Token"
            });

            userRepositoryMock.Setup(repo => repo.ModifyUser(UnitTestConstants.VALID_ID, It.IsAny<User>())).ReturnsAsync(new User
            {
                Id = UnitTestConstants.VALID_ID,
                Email = updatedUserDataRequest.Email,
                Password = "Password",
                Token = "Token",
                Username = updatedUserDataRequest.Username,
                FirstName = updatedUserDataRequest.FirstName,
                LastName = updatedUserDataRequest.LastName,
                RefreshToken = "Refresh Token"
            });

            var userService = new UserService(userRepositoryMock.Object);

            var serviceResult = await userService.ModifyUserData(UnitTestConstants.VALID_ID, updatedUserDataRequest);

            Assert.True(serviceResult.IsValid());
            Assert.Equal(UnitTestConstants.VALID_ID, serviceResult.Value.Id);
            Assert.Equal(updatedUserDataRequest.Email, serviceResult.Value.Email);
            Assert.Equal(updatedUserDataRequest.Username, serviceResult.Value.Username);
            Assert.Equal(updatedUserDataRequest.FirstName, serviceResult.Value.FirstName);
            Assert.Equal(updatedUserDataRequest.LastName, serviceResult.Value.LastName);

            userRepositoryMock.Verify(repo => repo.GetUserById(UnitTestConstants.VALID_ID), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser(UnitTestConstants.VALID_ID, It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Modify_RequestInvalidId_ShouldReturn_SocialCodeBadRequestErr()
        {
            var expectedServiceResult = new SocialCodeResult<UserDataResponse>
            {
                Value = null,
                ErrorMsg = "Invalid ID request (check request body & params)",
                ErrorTypes = SocialCodeErrorTypes.BadRequest
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var userService = new UserService(userRepositoryMock.Object);

            var serviceResult = await userService.ModifyUserData(UnitTestConstants.INVALID_ID, It.IsAny<UserDataRequest>());

            Assert.False(serviceResult.IsValid());
            Assert.Equal(expectedServiceResult.ErrorTypes, serviceResult.ErrorTypes);
            Assert.Equal(expectedServiceResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Null(expectedServiceResult.Value);
            userRepositoryMock.Verify(repo => repo.GetUserById(UnitTestConstants.INVALID_ID), Times.Never);
            userRepositoryMock.Verify(repo => repo.ModifyUser(UnitTestConstants.INVALID_ID, It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Modify_RequestInvalidUpdatedUserData_ShouldReturn_SocialCodeBadRequestErr()
        {
            var invalidUpdatedUserDataRequest = new UserDataRequest
            {
                Email = "test@test.com",
                Id = UnitTestConstants.VALID_ID,
                FirstName = "Test",
                LastName = "Dummy",
                Username = "Test"
            };
            
            var expectedServiceResult = new SocialCodeResult<UserDataResponse>
            {
                Value = null,
                ErrorMsg = "Invalid update user data request body",
                ErrorTypes = SocialCodeErrorTypes.BadRequest
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            
            var userService = new UserService(userRepositoryMock.Object);

            var serviceResult = await userService.ModifyUserData(UnitTestConstants.VALID_ID, invalidUpdatedUserDataRequest);
            
            Assert.False(serviceResult.IsValid());
            Assert.Equal(expectedServiceResult.ErrorTypes, serviceResult.ErrorTypes);
            Assert.Equal(expectedServiceResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Null(expectedServiceResult.Value);
            userRepositoryMock.Verify(repo => repo.GetUserById(UnitTestConstants.VALID_ID), Times.Never);
            userRepositoryMock.Verify(repo => repo.ModifyUser(UnitTestConstants.VALID_ID, It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Modify_UserNotFound_ShouldReturn_SocialCodeNotFoundErr()
        {
            var validUpdatedUserDataRequest = new UserDataRequest
            {
                Email = "test@test.com",
                Id = UnitTestConstants.VALID_ID,
                FirstName = "Test",
                LastName = "Dummy",
                Username = "@Test"
            };
            
            var expectedServiceResult = new SocialCodeResult<UserDataResponse>
            {
                Value = null,
                ErrorMsg = "User not found",
                ErrorTypes = SocialCodeErrorTypes.NotFound
            };
            
            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserById(UnitTestConstants.VALID_ID)).ReturnsAsync((User)null);
            
            var userService = new UserService(userRepositoryMock.Object);

            var serviceResult = await userService.ModifyUserData(UnitTestConstants.VALID_ID, validUpdatedUserDataRequest);
            
            Assert.False(serviceResult.IsValid());
            Assert.Equal(expectedServiceResult.ErrorTypes, serviceResult.ErrorTypes);
            Assert.Equal(expectedServiceResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Null(expectedServiceResult.Value);
            userRepositoryMock.Verify(repo => repo.GetUserById(UnitTestConstants.VALID_ID), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser(UnitTestConstants.VALID_ID, It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Modify_ModifyRepositoryReturnsNull_ShouldReturn_SocialCodeGenericErr()
        {
            var validUpdatedUserDataRequest = new UserDataRequest
            {
                Email = "test@test.com",
                Id = UnitTestConstants.VALID_ID,
                FirstName = "Test",
                LastName = "Dummy",
                Username = "@Test"
            };
            
            var expectedServiceResult = new SocialCodeResult<UserDataResponse>
            {
                Value = null,
                ErrorMsg = "Failed to update user data",
                ErrorTypes = SocialCodeErrorTypes.Generic
            };
            
            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserById(UnitTestConstants.VALID_ID)).ReturnsAsync(new User
            {
                Id = UnitTestConstants.VALID_ID,
                Email = "user@email.com",
                Password = "Password",
                Username = "@Username",
                FirstName = "FirstName",
                LastName = "LastName",
                Token = "Token",
                RefreshToken = "Refresh Token"
            });
            userRepositoryMock.Setup(repo => repo.ModifyUser(UnitTestConstants.VALID_ID, It.IsAny<User>())).ReturnsAsync((User)null);
            
            var userService = new UserService(userRepositoryMock.Object);

            var serviceResult = await userService.ModifyUserData(UnitTestConstants.VALID_ID, validUpdatedUserDataRequest);
            
            Assert.False(serviceResult.IsValid());
            Assert.Equal(expectedServiceResult.ErrorTypes, serviceResult.ErrorTypes);
            Assert.Equal(expectedServiceResult.ErrorMsg, serviceResult.ErrorMsg);
            Assert.Null(expectedServiceResult.Value);
            userRepositoryMock.Verify(repo => repo.GetUserById(UnitTestConstants.VALID_ID), Times.Once);
            userRepositoryMock.Verify(repo => repo.ModifyUser(UnitTestConstants.VALID_ID, It.IsAny<User>()), Times.Once); 
        }
    }
}