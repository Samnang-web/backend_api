using Backend.DTOs;
using Backend.Models;
using Backend.Repository;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly TokenServices _tokenServices;
        public AuthController (IUserRepository userRepo, TokenServices tokenServices)
        {
            _userRepo = userRepo;
            _tokenServices = tokenServices;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existing = await _userRepo.GetByUsername(dto.Username);
            if (existing != null) return BadRequest("Username taken");

            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            await _userRepo.Create(new User { Username = dto.Username, PasswordHash = hash });

            return Ok("Registered");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userRepo.GetByUsername(dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Username or password is Incorrect!");

            var token = _tokenServices.CreateToken(user);
            return Ok(new { token });
        }
    }
}
