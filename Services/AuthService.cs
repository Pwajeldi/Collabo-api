using Collabo_app.Database;
using Collabo_app.Dto;
using Collabo_app.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Collabo_app.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly UserDbContext _context;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<AuthService> logger, UserDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        //ACCESS-TOKEN GENERATOR
        public async Task<string> GenerateAccessToken(ApplicationUser User)
        {
            var userRoles = await _userManager.GetRolesAsync(User);
            var authClaims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, User.UserId.ToString()),
                new (ClaimTypes.Email, User.Email ?? throw new Exception("Empty email")),
                new Claim(ClaimTypes.Name, User.UserName ?? throw new Exception("Empty username")),
            };
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new Exception("Jwt key not found")));
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], audience: _configuration["Jwt:Audience"],
                claims: authClaims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:LifeTime"] ?? throw new Exception("Token lifetime not set"))),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //REFESH-TOKEN GENERATOE
        public async Task<string> GenerateRefreshToken(string userId)
        {
            var randomNum = new Byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNum);
            var refreshToken = Convert.ToBase64String(randomNum);
            return refreshToken;
        }

        // LOGIN SERVICE
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user == null)
            {
                throw new Exception("Wrong Email");
            }
            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                throw new Exception("Incorrect Password");
            }
            var accessToken = await GenerateAccessToken(user);
            var refreshtoken = await GenerateRefreshToken(user.UserId.ToString());
            var Newrefresh = new RefreshTokens
            {
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false,
                Expiry = DateTime.UtcNow.AddDays(3),
                RefreshToken = refreshtoken,
                UserId = user.Id
            };
            await _context.RefreshTokensTab.AddAsync(Newrefresh);
            await _context.SaveChangesAsync();
            _logger.LogInformation("New Registration: {email}", user.Email);
            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshtoken,
            };
        }

        //REFRESH SERVICE
        public async Task<RefreshResponseDto> RefreshAsync(RequestRefreshDto request)
        {
            var storedtoken = await _context.RefreshTokensTab.FirstOrDefaultAsync(t => t.RefreshToken == request.RefreshToken);
            if(storedtoken == null || storedtoken.IsRevoked == true || storedtoken.Expiry <= DateTime.UtcNow)
            {
                throw new Exception("Invalid or expired token");
            }
            var user = await _userManager.FindByIdAsync(storedtoken.UserId!);
           
            storedtoken.IsRevoked = true;
            await _context.SaveChangesAsync();
            var newAccessToken = await GenerateAccessToken(user ?? throw new Exception("No user to generate access-token"));
            var newRefreshToken = await GenerateRefreshToken(user.UserId.ToString());
            _context.RefreshTokensTab.Remove(storedtoken);
            var Newrefresh = new RefreshTokens
            {
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false,
                Expiry = DateTime.UtcNow.AddDays(3),
                RefreshToken = newRefreshToken,
                UserId = user.Id
            };
            await _context.RefreshTokensTab.AddAsync(Newrefresh);
            await _context.SaveChangesAsync();
            return (new RefreshResponseDto { RefreshToken = newRefreshToken, AccessToken = newAccessToken});
        }

        // SIGN-UP SERVICE
        public async Task SignUpAsync(SignUpRequestDto dto)
        {
            if(await _userManager.FindByEmailAsync(dto.Email) != null)
            {
                throw new Exception("User with this email already exists");
            }
            if(dto.Password != dto.ConfirmPassword)
            {
                throw new Exception("PAsswords don't match");
            }
            var appUser = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                FullName = dto.FirstName + " " + dto.LastName,
                Role = "Member",
            };
            var result = await _userManager.CreateAsync(appUser, dto.Password);
            if (!result.Succeeded)
            {
                var error = result.Errors.Select(e => e.Description);
                throw new Exception(error.ToString()); 
            }
            await _userManager.AddToRoleAsync(appUser, appUser.Role);
            _logger.LogInformation("New Registration: {email}", appUser.Email);
        }
    }
}
