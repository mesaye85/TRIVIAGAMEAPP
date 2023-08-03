using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using TriviaGameApp.Models;

[ApiController]
[Route("api/v1/users")]
public class UserController : ControllerBase
{
    private readonly IMongoCollection<User> _users;
    private readonly IConfiguration _configuration;

    public UserController(IMongoDatabase database, IConfiguration configuration)
    {
        _users = database.GetCollection<User>("TriviaGame");
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDto userDto)
    {
        var user = new User
        {
            Email = userDto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
        };

        await _users.InsertOneAsync(user);

        return Ok(new { Message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserDto userDto)
    {
        var user = await _users.Find(u => u.Email == userDto.Email).FirstOrDefaultAsync();

        if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password))
        {
            return Unauthorized(new { Message = "Invalid email or password" });
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:Secret"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim("id", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            // You can add more claims if needed
        }),
            Expires = DateTime.UtcNow.AddHours(6), // Token expiration, you can set it to whatever you want
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new
        {
            token = tokenHandler.WriteToken(token),
            message = "Login successful"
        });
    }
}
