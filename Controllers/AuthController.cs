using Collabo_app.Dto;
using Collabo_app.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Collabo_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp(SignUpRequestDto dto)
        {
            try
            {
                await _authService.SignUpAsync(dto);
                return Ok($"User {dto.Email} Successfully created");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message );
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            try
            {
                var tokens = await _authService.LoginAsync(dto);
                return Ok(tokens);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RequestRefreshDto dto)
        {
            try
            {
               var newTokens =  await _authService.RefreshAsync(dto);
                return Ok(newTokens);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
