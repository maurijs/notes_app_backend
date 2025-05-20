// Controllers/AuthController.cs
using backend.Models;
using backend.Services;
using backend.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
                    return BadRequest("El nombre de usuario y la contraseña son obligatorios.");
                bool registered = await _authService.RegisterAsync(dto.Username, dto.Password);
                if (!registered)
                    return BadRequest("El nombre de usuario ya está en uso.");

                return Ok("Usuario registrado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en registro: " + ex.Message);
                return StatusCode(500, "Error interno en el servidor.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            { 
                
                if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
                    return BadRequest("El nombre de usuario y la contraseña son obligatorios.");
                var user = await _authService.AuthenticateAsync(dto.Username, dto.Password);
                if (user == null)
                    return Unauthorized("Usuario o contraseña incorrectos.");

                var token = _authService.GenerateJwtToken(user);
                return Ok(new { token });
            } catch (Exception ex)
            {
                Console.WriteLine("Error en login: " + ex.Message);
                return StatusCode(500, "Error interno en el servidor.");
            }
        }
    }

}
