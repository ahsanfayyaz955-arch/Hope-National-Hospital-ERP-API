using Hope_National_Hospital.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hope_National_Hospital.Application;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;


namespace Hope_National_Hospital.Infrastructure.JWT
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public Task<string>GenerateToken(string UserId, string Emails, string FullName, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                  new Claim(JwtRegisteredClaimNames.Sub, UserId),
                  new Claim(JwtRegisteredClaimNames.Email,Emails),
                  new Claim(ClaimTypes.Name, FullName),
                  new Claim(ClaimTypes.NameIdentifier,UserId),
                  new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            // Add Roles To JWT 
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var Credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(

                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: Credentials);

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));

        }
    }
}
