using HotelApp.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelApp.Domain.Services
{
    public class JwtService : IJwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;

        //Constuructor ile appsettings.json'dan JWT ayarlarını alıyoruz
        public JwtService(string secretKey, string issuer, string audience, int expirationMinutes)
        {
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
            _expirationMinutes = expirationMinutes;
        }

        public string GenerateToken(User user, out DateTime expireAt)
        {
            //1. Claims (token içindeki kullanıcı bilgileri)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            // Roller varsa ekle
            foreach (var role in user.Roles)

                claims.Add(new Claim(ClaimTypes.Role, role.Name));

            // 2. İmzalama anahtarı
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. Token oluştur
            expireAt = DateTime.UtcNow.AddMinutes(_expirationMinutes);
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expireAt,
                signingCredentials: creds
            );

            // 4. Token'ı string olarak döndür
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
