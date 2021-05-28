using System.Threading.Tasks;
using SocialCode.API.Requests;
using SocialCode.API.Requests.Users.Auth;
using SocialCode.API.Requests.Users.Register;

namespace SocialCode.API.Services.Auth
{
    public interface IAuthService
    {
        Task<SocialCodeResult<AuthResponse>> LogIn(LoginRequest loginRequest);
        Task<SocialCodeResult<AuthResponse>> Register(RegisterRequest registerRequest);
        Task<SocialCodeResult<AuthResponse>> RefreshToken(RefreshTokenRequest refreshRefreshTokenRequest);
    }
}