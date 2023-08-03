using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using TriviaGameApp.Models;
using Microsoft.Extensions.Logging;
using TriviaGameApp.Data;

[ApiController]
[Route("api/v1/users")]
public class UserController : ControllerBase
{

    private readonly TriviaDBContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserController> _logger;

    public UserController(TriviaDBContext context, IConfiguration configuration, ILogger<UserController> logger) // added logger parameter
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDto userDto)
    {
        try
        {
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == userDto.Email);
            if (existingUser != null)
                return BadRequest(new { Message = "Email is already registered" });

            var user = new User
            {
                Email = userDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while registering user");

            return StatusCode(500, new { Message = "An error occurred, please try again" });
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(UserDto userDto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userDto.Email);

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
            }),
            Expires = DateTime.UtcNow.AddHours(6),
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
