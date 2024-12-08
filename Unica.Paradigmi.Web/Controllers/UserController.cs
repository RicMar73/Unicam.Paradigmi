using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Unicam.Paradigmi.Models;
using Unicam.Paradigmi.Models.Context;
using Unicam.Paradigmi.Models.DTOs;
using Unicam.Paradigmi.Models.Models;

namespace Unicam.Paradigmi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Endpoint per la creazione di un utente
        [HttpPost("register")]
        public IActionResult Register(UserRegistrationDto registrationDto)
        {
            if (registrationDto == null)
                return BadRequest("I dati dell'utente sono mancanti.");

            // Validazione: controlla che l'email e la password non siano vuote
            if (string.IsNullOrEmpty(registrationDto.Email))
                return BadRequest("L'email non può essere vuota.");

            if (string.IsNullOrEmpty(registrationDto.Password))
                return BadRequest("La password non può essere vuota.");

            // Validazione dell'email
            if (!new EmailAddressAttribute().IsValid(registrationDto.Email))
                return BadRequest("L'email non è valida.");

            // Verifica che l'email non sia già presente nel database
            if (_context.Users.Any(u => u.Email == registrationDto.Email))
                return BadRequest("L'email è già in uso.");

            // Creazione dell'entità User a partire dal DTO
            var user = new User
            {
                Email = registrationDto.Email,
                FirstName = registrationDto.FirstName,
                LastName = registrationDto.LastName,
                // Hasher della password
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password)
            };

            // Salvataggio nel db
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { Message = "Utente creato con successo.", UserId = user.Id });
        }

        // Endpoint per l'autenticazione
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Credenziali non valide.");

            // Generare il token JWT
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("throw-error")]
        public async Task<IActionResult> ThrowError()
        {
            throw new Exception("Errore simulato per testare il middleware");
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
