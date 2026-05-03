using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SoundSphereApi.DTOs.Auth;
using SoundSphereApi.Models.Identity;
using SoundSphereApi.Repositories.Interfaces;
using SoundSphereApi.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SoundSphereApi.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(
            IRepository<User> userRepository,
            IRepository<Role> roleRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            var existingUserByEmail = await _userRepository.FindAsync(u => u.Email == registerDto.Email);
            if (existingUserByEmail != null)
            {
                return "User with this email already exists.";
            }

            var existingUserByUserName = await _userRepository.FindAsync(u => u.UserName == registerDto.UserName);
            if (existingUserByUserName != null)
            {
                return "Username already exists.";
            }

            var userRole = await _roleRepository.FindAsync(r => r.Name == "User");

            if (userRole == null)
            {
                userRole = new Role
                {
                    Name = "User"
                };

                await _roleRepository.AddAsync(userRole);
                await _roleRepository.SaveChangesAsync();
            }

            var user = new User
            {
                FullName = registerDto.FullName,
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                RoleId = userRole.Id,
                IsActive = true
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, registerDto.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return "User registered successfully.";
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.FindAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                return null;
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                loginDto.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var role = await _roleRepository.GetByIdAsync(user.RoleId);

            var token = GenerateJwtToken(user, role?.Name ?? "User");

            return new AuthResponseDto
            {
                UserId = user.Id,
                Token = token,
                Email = user.Email,
                UserName = user.UserName,
                Role = role?.Name ?? "User"
            };

        }

        public async Task<AuthResponseDto?> TryRestoreSessionFromTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2)
                };

                tokenHandler.ValidateToken(token, validationParams, out SecurityToken validatedToken);

                var jwt = (JwtSecurityToken)validatedToken;
                var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return null;
                }

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || !user.IsActive)
                {
                    return null;
                }

                var role = await _roleRepository.GetByIdAsync(user.RoleId);

                return new AuthResponseDto
                {
                    UserId = user.Id,
                    Token = token,
                    Email = user.Email,
                    UserName = user.UserName,
                    Role = role?.Name ?? "User"
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var role = await _roleRepository.GetByIdAsync(user.RoleId);

            return new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                Role = role?.Name ?? "User",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<string> UpdateUserProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return "User not found.";
            }

            // Check if new username is taken by someone else
            if (user.UserName != dto.UserName)
            {
                var existing = await _userRepository.FindAsync(u => u.UserName == dto.UserName);
                if (existing != null)
                {
                    return "Username already taken.";
                }
            }

            user.FullName = dto.FullName;
            user.UserName = dto.UserName;
            user.ProfileImageUrl = dto.ProfileImageUrl;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return "Profile updated successfully.";
        }

        private string GenerateJwtToken(User user, string roleName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
