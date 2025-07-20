using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HairpinStore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly SignInManager<IdentityUser> _signInManager;
	private readonly IConfiguration _configuration;

	public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
		IConfiguration configuration)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_configuration = configuration;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterDto model)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var user = new IdentityUser
		{
			UserName = model.Email,
			Email = model.Email
		};

		var result = await _userManager.CreateAsync(user, model.Password);

		if (result.Succeeded)
		{
			// Assign Customer role to new users by default
			await _userManager.AddToRoleAsync(user, "Customer");
			
			var token = await GenerateJwtToken(user);
			return Ok(new AuthResponseDto
			{
				Token = token,
				Email = user.Email,
				UserId = user.Id
			});
		}

		foreach (var error in result.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}

		return BadRequest(ModelState);
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginDto model)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var user = await _userManager.FindByEmailAsync(model.Email);
		if (user == null)
		{
			return Unauthorized("Invalid Credentials");
		}

		var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
		if (result.Succeeded)
		{
			var token = await GenerateJwtToken(user);
			return Ok(new AuthResponseDto
			{
				Token = token,
				Email = user.Email!,
				UserId = user.Id
			});
		}

		return Unauthorized("Invalid Credentials");
	}

	private async Task<string> GenerateJwtToken(IdentityUser user)
	{
		var jwtSettings = _configuration.GetSection("JwtSettings");
		var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id),
			new Claim(ClaimTypes.Email, user.Email!),
			new Claim(ClaimTypes.Name, user.UserName!)
		};

		var roles = await _userManager.GetRolesAsync(user);
		claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSettings["ExpirationInDays"])),
			Issuer = jwtSettings["Issuer"],
			Audience = jwtSettings["Audience"],
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		};

		var tokenHandler = new JwtSecurityTokenHandler();
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}
}

// DTOs
public class RegisterDto
{
	public string Email { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginDto
{
	public string Email { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
	public string Token { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string UserId { get; set; } = string.Empty;
}