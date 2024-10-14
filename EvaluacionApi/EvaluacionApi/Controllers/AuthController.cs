// Controllers/AuthController.cs
using EvaluacionApi.Models;
using EvaluacionApi.Models;
using EvaluacionApi.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OtpNet;
using QRCoder;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GestionSolicitudesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // POST: api/Auth/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Nombres = model.Nombres,
                Apellidos = model.Apellidos,
                FechaNacimiento = model.FechaNacimiento,
                Genero = model.Genero
                // FotografiaUrl se puede agregar posteriormente
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

            // Generar clave secreta para TOTP
            var secret = KeyGeneration.GenerateRandomKey(20);
            user.TOTPSecret = Base32Encoding.ToString(secret);
            await _userManager.UpdateAsync(user);

            // Generar código QR para que el usuario configure su autenticador
            var totp = new Totp(secret);
            var uri = $"otpauth://totp/GestionSolicitudes:{user.Email}?secret={user.TOTPSecret}&issuer=GestionSolicitudes";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            var qrCodeBase64 = Convert.ToBase64String(qrCodeImage);

            return Ok(new
            {
                Message = "Usuario creado exitosamente. Escanea el siguiente QR con tu aplicación de autenticación.",
                QrCodeImage = qrCodeBase64
            });
        }

        // POST: api/Auth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Usuario o contraseña inválidos.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Usuario o contraseña inválidos.");

            if (user.TwoFactorEnabled)
            {
                // Aquí podrías implementar la lógica para solicitar el código 2FA
                // Por simplicidad, asumiremos que el usuario envía el código en el mismo request
                if (string.IsNullOrEmpty(model.TOTPCode))
                    return BadRequest("Se requiere el código de autenticación de doble factor.");

                var totp = new Totp(Base32Encoding.ToBytes(user.TOTPSecret));
                var isValid = totp.VerifyTotp(model.TOTPCode, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

                if (!isValid)
                    return Unauthorized("Código de autenticación de doble factor inválido.");
            }

            // Generar JWT
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Token = token
            });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationInMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // POST: api/Auth/Verify2FA
        [HttpPost("Verify2FA")]
        public async Task<IActionResult> Verify2FA([FromBody] Verify2FAViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Usuario no encontrado.");

            var totp = new Totp(Base32Encoding.ToBytes(user.TOTPSecret));
            var isValid = totp.VerifyTotp(model.TOTPCode, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

            if (!isValid)
                return Unauthorized("Código de autenticación de doble factor inválido.");

            // Habilitar 2FA si no está habilitado
            if (!user.TwoFactorEnabled)
            {
                user.TwoFactorEnabled = true;
                await _userManager.UpdateAsync(user);
            }

            // Generar JWT
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Token = token
            });
        }
    }
}
