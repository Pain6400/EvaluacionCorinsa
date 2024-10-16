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

namespace EvaluacionApi.Controllers
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
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Generar clave secreta para TOTP
                var secret = KeyGeneration.GenerateRandomKey(20);
                var TOTPSecret = Base32Encoding.ToString(secret);

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nombres = model.Nombres,
                    Apellidos = model.Apellidos,
                    FechaNacimiento = model.FechaNacimiento.ToUniversalTime(),
                    Genero = model.Genero,
                    FotografiaUrl = "https://tu-dominio.com/images/default-profile.png", // Asigna una URL predeterminada
                    TOTPSecret = TOTPSecret,
                    // Habilitar 2FA
                    TwoFactorEnabled = true
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

                // Generar código QR para que el usuario configure su autenticador
                var totp = new Totp(secret);
                var uri = $"otpauth://totp/EvaluacionApi:{user.Email}?secret={user.TOTPSecret}&issuer=EvaluacionApi";

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
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }


        // POST: api/Auth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return Unauthorized("Usuario o contraseña inválidos.");

                var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!passwordValid)
                    return Unauthorized("Usuario o contraseña inválidos.");

                if (user.TwoFactorEnabled)
                {
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
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

        private string GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

            // Obtener los roles del usuario
            var roles = _userManager.GetRolesAsync(user).Result;

            // Crear los claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id), // Asegúrate de que esto es el Id
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Agregar los roles como claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Crear la clave de seguridad
            var signingKey = new SymmetricSecurityKey(key);

            // Crear las credenciales de firma
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // Crear el token
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationInMinutes"])),
                signingCredentials: signingCredentials
            );

            // Devolver el token en formato string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
