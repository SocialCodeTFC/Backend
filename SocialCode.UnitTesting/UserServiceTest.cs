using Xunit;

namespace SocialCode.UnitTesting
{
    public class UserServiceTest
    {
        [Fact]
        public void GetById_ValidID_ShouldReturn_SocialCodeResult()
        {
        }
        
        [Fact]
        public void GetById_NotValidId_ShouldReturn_SocialCodeBadRequestErr()
        {
        }
        
        [Fact]
        public void GetById_IdNotFound_ShouldReturn_SocialCodeNotFoundErr()
        {
        }
        
        
        [Fact]
        public void Delete_ValidID_ShouldReturn_SocialCodeResult()
        {
        }
        
        [Fact]
        public void Delete_InvalidId_ShouldReturn_SocialCodeBadRequestErr()
        {
        }
        
        [Fact]
        public void Delete_IdNotFound_ShouldReturn_SocialCodeNotFoundErr()
        {
        }
        
        [Fact]
        public void Delete_DeleteRepositoryReturnsNull_ShouldReturn_SocialCodeGenericErr()
        {
        }
        
        
        [Fact]
        public void Modify_ValidUpdatedData_ShouldReturn_SocialCodeResult()
        {
        }

        [Fact]
        public void Modify_RequestInvalidId_ShouldReturn_SocialCodeBadRequestErr()
        {
            
        }
        
        [Fact]
        public void Modify_RequestInvalidUpdatedUserData_ShouldReturn_SocialCodeBadRequestErr()
        {
            
        }
        
        [Fact]
        public void Modify_IdNotFound_ShouldReturn_SocialCodeBadRequestErr()
        {
            
        }
        
        [Fact]
        public void Modify_ModifyRepositoryReturnsNull_ShouldReturn_SocialCodeGenericErr()
        {
        }
    }
}