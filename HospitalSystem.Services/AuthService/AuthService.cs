using HospitalSystem.DataAccess.Interfaces;
using HospitalSystem.Services.AuthService.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HospitalSystem.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly string _secretKey;
        private readonly double _tokenExpirationMinutes;
        private readonly IAuthRepository _authRepository;

        public AuthService(IConfiguration configuration, IAuthRepository authRepository)
        {
            // Retrieve JWT secret key and token expiration settings from configuration
            _secretKey = configuration["JwtSettings:SecretKey"];
            _tokenExpirationMinutes = double.Parse(configuration["JwtSettings:ExpirationInMinutes"]);
            _authRepository = authRepository;
        }

        /// <summary>
        /// Authenticates user.
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>The generated JWT token as a string.</returns>
        public async Task<string?> AuthenticateUserAsync(string username, string password)
        {
            var user = await _authRepository.GetUserByUsernameAsync(username);

            // Validate the user's password 
            if (user != null && ValidatePassword(password, user.Password))
            {
                // Generate JWT token and return it
                return GenerateJwtToken(user.Id.ToString(), user.Username);
            }

            return null; // Authentication failed
        }

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="username">The user's username.</param>
        /// <returns>The generated JWT token as a string.</returns>
        public string GenerateJwtToken(string userId, string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            // Set the claims for the user in the token
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, username)
        };

            // Configure the token descriptor with the claims, expiration time, and signing credentials
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_tokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Create and write the token using the token descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Password validation
        private bool ValidatePassword(string enteredPassword, string storedPasswordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(enteredPassword, storedPasswordHash);
        }
    }
}
