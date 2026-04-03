using Collabo_app.Dto;
using Collabo_app.Models;


namespace Collabo_app.Services
{
    public interface IAuthService
    {
        Task SignUpAsync(SignUpRequestDto dto);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        Task<string> GenerateAccessToken(ApplicationUser User);
        Task<string> GenerateRefreshToken(string UserId);
        Task<RefreshResponseDto> RefreshAsync(RequestRefreshDto request);
    }
}
