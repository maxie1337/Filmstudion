using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using API.Interfaces;

namespace API.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;
    
        public JwtService(IConfiguration config)
        {
            _config = config;
        }
    
        public string GenerateToken(IUser user)
{
    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    // Try to get film studio id using "filmStudioId" or "UserId" (case-insensitive)
    var prop = user.GetType().GetProperty("filmStudioId", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
    if (prop == null)
    {
        // fallback: try UserId property
        prop = user.GetType().GetProperty("UserId", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
    }
    if (prop != null)
    {
        var idValue = prop.GetValue(user)?.ToString();
        if (!string.IsNullOrEmpty(idValue))
        {
            claims.Add(new Claim("UserId", idValue));
        }
    }
    
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    int expiresInMinutes = int.Parse(_config["Jwt:ExpiresInMinutes"]);

    var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddMinutes(expiresInMinutes),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
    }
}
