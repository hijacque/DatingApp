using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context) : BaseAPIController
{
    [HttpPost("register")] // endpoint: /api/account/register
    public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto){
        if (await UserExists(registerDto.Username)) return BadRequest("Username is taken.");

        using var hmac = new HMACSHA256();

        var user = new AppUser{
            UserName = registerDto.Username,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key,
        };

        context.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    private async Task<bool> UserExists(string username) {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
