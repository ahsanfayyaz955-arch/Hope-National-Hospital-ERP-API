
using Hope_National_Hospital.Application.Dtos;
using Hope_National_Hospital.Application.Interfaces;
using Hope_National_Hospital.Domain.Entities;
using Hope_National_Hospital.Infrastructure.Data;
using Hope_National_Hospital.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Hope_National_Hospital.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IAuditService _auditService;
        private readonly AppDbContext _context;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtService jwtService, IAuditService auditService, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _auditService = auditService;
            _context = context;
        }






        // =========================================================
        // REGISTER
        // =========================================================
        public async Task<AuthResponseDto> RegisterAsync(
            RegisterDto dto)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new ArgumentException(
                    "Full name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException(
                    "Email is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException(
                    "Password is required.");
            }


            // Check existing email
            var existingUser =
                await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                throw new ArgumentException(
                    "Email already exists.");
            }


            // Create user
            var user = new ApplicationUser
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                UserName = dto.Email.Trim()
            };


            var result =
                await _userManager.CreateAsync(
                    user,
                    dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        x => x.Description));

                throw new ArgumentException(errors);
            }


            // Assign default role
            var roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    "Patient");

            if (!roleResult.Succeeded)
            {
                // Rollback user if role assignment fails
                await _userManager.DeleteAsync(user);

                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(
                        x => x.Description));

                throw new InvalidOperationException(
                    $"Unable to assign default role. {errors}");
            }


            // Get roles
            var roles =
                await _userManager.GetRolesAsync(user);


            // Generate JWT
            var token =
                await _jwtService.GenerateToken(
                    user.Id,
                    user.Email!,
                    user.FullName,
                    roles);

            // REGISTER
            await _auditService.LogAsync(
                "REGISTER",
                "User",
                user.Id,
                $"User '{user.Email}' registered successfully."
            );


            return new AuthResponseDto
            {
                Success = true,

                Message =
                    "Registration successful.",

                Token =
                    token,

                Expiration =
                    DateTime.UtcNow.AddMinutes(60)
            };



        }

      


        // =========================================================
        // LOGIN
        // =========================================================
        public async Task<AuthResponseDto> LoginAsync(
            LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException(
                    "Email is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException(
                    "Password is required.");
            }


            // Find user
            var user =
                await _userManager.FindByEmailAsync(
                    dto.Email.Trim());


            // Do NOT reveal whether email exists
            if (user == null)
            {
                return InvalidLoginResponse();
            }


            // Check password
            var result =
                await _signInManager.CheckPasswordSignInAsync(
                    user,
                    dto.Password,
                    lockoutOnFailure: true);


            if (!result.Succeeded)
            {
                return InvalidLoginResponse();
            }


            // Get roles
            var roles =
                await _userManager.GetRolesAsync(user);


            // Generate JWT
            var token =
                await _jwtService.GenerateToken(
                    user.Id,
                    user.Email!,
                    user.FullName,
                    roles);

            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false

            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();


            // LOGIN
            await _auditService.LogAsync(
                "LOGIN",
                "User",
                user.Id,
                $"User '{user.Email}' logged in successfully."
            );

            return new AuthResponseDto
            {
                Success = true,

                Message =
                    "Login successful.",

                Token =
                    token,

                RefreshToken =refreshToken ,

                Expiration =
                    DateTime.UtcNow.AddMinutes(60)
            };

          
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentException(
                    "Refresh token is required.");
            }

            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(r =>
                    r.Token == refreshToken);

            if (storedToken == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid refresh token."
                };
            }

            if (storedToken.IsRevoked)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Refresh token has been revoked."
                };
            }

            if (storedToken.ExpiresAt <= DateTime.UtcNow)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Refresh token has expired."
                };
            }

            var user = await _userManager.FindByIdAsync(
                storedToken.UserId);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            var roles =
                await _userManager.GetRolesAsync(user);

            var newAccessToken =
                await _jwtService.GenerateToken(
                    user.Id,
                    user.Email!,
                    user.FullName,
                    roles);

            // Rotate refresh token
            var newRefreshToken =
                GenerateRefreshToken();

            storedToken.IsRevoked = true;

            storedToken.RevokedAt =
                DateTime.UtcNow;

            storedToken.ReplacedByToken =
                newRefreshToken;

            var newRefreshTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,

                UserId = user.Id,

                CreatedAt =
                    DateTime.UtcNow,

                ExpiresAt =
                    DateTime.UtcNow.AddDays(7),

                IsRevoked = false
            };

            _context.RefreshTokens.Add(
                newRefreshTokenEntity);

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                "REFRESH_TOKEN",
                "User",
                user.Id,
                $"Access token refreshed for '{user.Email}'."
            );

            return new AuthResponseDto
            {
                Success = true,

                Message =
                    "Token refreshed successfully.",

                Token =
                    newAccessToken,

                RefreshToken =
                    newRefreshToken,

                Expiration =
                    DateTime.UtcNow.AddMinutes(60)
            };
        }


        // =========================================================
        // INVALID LOGIN RESPONSE
        // =========================================================
        private static AuthResponseDto
            InvalidLoginResponse()
        {
            return new AuthResponseDto
            {
                Success = false,

                Message =
                    "Invalid email or password."
            };
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}

