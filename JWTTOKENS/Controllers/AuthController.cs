using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace JWTTOKENS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly string ValidUser;
        private readonly string ValidPass;
        private readonly string JwtKey;
        private readonly string JwtIssuer;
        private readonly string JwtAudience;

        public AuthController(IConfiguration config)
        {
            ValidUser = config["JwtSettings:User"]!;
            ValidPass = config["JwtSettings:Password"]!;
            JwtKey = config["JwtSettings:Key"]!;
            JwtIssuer = config["JwtSettings:Issuer"]!;
            JwtAudience = config["JwtSettings:Audience"]!;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { message = "Username and Password required." });

            if (req.Username != ValidUser || req.Password != ValidPass)
                return Unauthorized(new { message = "Invalid credentials." });

            var token = GenerateJwtToken(req.Username);
            return Ok(new { access_token = token, token_type = "Bearer" });
        }

        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("role", "admin")
            };

            var token = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public sealed class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
