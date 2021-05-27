using System.Threading.Tasks;
using SocialCode.API.Services.Requests;
using SocialCode.API.Services.Requests.Users.Auth;
using SocialCode.API.Services.Requests.Users.Register;

namespace SocialCode.API.Services.Auth
{
    public interface IAuthService
    {
        Task<SocialCodeResult<AuthResponse>> LogIn(LoginRequest loginRequest);
        Task<SocialCodeResult<AuthResponse>> Register(RegisterRequest registerRequest);
        Task<SocialCodeResult<AuthResponse>> RefreshToken(RefreshTokenRequest refreshRefreshTokenRequest);
    }
}