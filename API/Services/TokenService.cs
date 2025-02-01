using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user) {
        // Retrieve secured string TokenKey from appsettings.json
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot access TokenKey from appsettings.json");

        // VAlidate if token length satisfies security protocol
        if (tokenKey.Length < 64) throw new Exception("Your TokenKey neeeds to be longer than 64 characters.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        // Define the payload via Claims
        var claims = new List<Claim> {
            new(ClaimTypes.NameIdentifier, user.UserName)
        };
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = credentials,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
