using Commons.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Commons.Services
{
    public static class ProfileAutheticationTokenServices
    {
        public static string _secret = "fedaf7d8863b48e197b9287d492b708e";
        private static byte[] _key = Encoding.ASCII.GetBytes(_secret);
        private static JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler();
        private static int _tokenAddMinutes = 30;

        public static string GenerateToken(Profile profile)
        {
            var subject = GetClaimsIdentity(profile);
            var tokenDescriptor = GetSecurityTokenDescriptor(subject);
            var token = _handler.CreateToken(tokenDescriptor);

            return _handler.WriteToken(token);
        }

        public static JwtSecurityToken ReadToken(string token)
        {

            if (!_handler.CanReadToken(token))
            {
                return default;
            }

            return _handler.ReadJwtToken(token);
        }

        private static ClaimsIdentity GetClaimsIdentity(Profile profile)
        {
            var subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, profile.Name)
            });

            foreach (var role in profile.Roles)
            {
                subject.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return subject;
        }

        private static SecurityTokenDescriptor GetSecurityTokenDescriptor(ClaimsIdentity subject)
        {
            return new SecurityTokenDescriptor
            {
                Subject = subject,
                Expires = DateTime.UtcNow.AddMinutes(_tokenAddMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
            };
        }
    }
}
