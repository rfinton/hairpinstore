using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HairpinStore.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class AdminController : ControllerBase
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;

	public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
	{
		_userManager = userManager;
		_roleManager = roleManager;
	}

	[HttpGet("users")]
	public async Task<IActionResult> GetAllUsers()
	{
		var users = await _userManager.Users
			.Select(u => new
			{
				u.Id,
				u.Email,
				u.UserName,
				u.EmailConfirmed,
				CreatedDate = u.LockoutEnabled
			})
			.ToListAsync();

		return Ok(users);
	}

	[HttpGet("users/{userId}/roles")]
	public async Task<IActionResult> GetUserRoles(string userId)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return NotFound("User not found");

		var roles = await _userManager.GetRolesAsync(user);
		return Ok(new { UserId = userId, Email = user.Email, Roles = roles });
	}

	[HttpPost("users/{userId}/roles")]
	public async Task<IActionResult> AssignRoleToUser(string userId, [FromBody] AssignRoleDto model)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return NotFound("User not found");

		var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
		if (!roleExists)
			return BadRequest("Role does not exists");

		var isInRole = await _userManager.IsInRoleAsync(user, model.RoleName);
		if (isInRole)
			return BadRequest("User already has this role");

		var result = await _userManager.AddToRoleAsync(user, model.RoleName);
		if (result.Succeeded)
			return Ok($"Role '{model.RoleName}' assigned to user '{user.Email}'");

		return BadRequest(result.Errors);
	}

	[HttpDelete("users/{userId}/roles/{roleName}")]
	public async Task<IActionResult> RemoveRoleFromUser(string userId, string roleName)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return NotFound("User not found");

		var isInRole = await _userManager.IsInRoleAsync(user, roleName);
		if (!isInRole)
			return BadRequest("User does not have this role");

		// Prevent removing Adminstrator role from the last admin
		if (roleName.Equals("Administrator", StringComparison.OrdinalIgnoreCase))
		{
			var adminUsers = await _userManager.GetUsersInRoleAsync("Administrator");
			if (adminUsers.Count <= 1)
				return BadRequest("Cannot remove Administrator role from the last administrator");
		}

		var result = await _userManager.RemoveFromRoleAsync(user, roleName);
		if (result.Succeeded)
			return Ok($"Role '{roleName}' removed from user '{user.Email}'");

		return BadRequest(result.Errors);
	}

	[HttpGet("roles")]
	public async Task<IActionResult> GetAllRoles()
	{
		var roles = await _roleManager.Roles
			.Select(r => new { r.Id, r.Name })
			.ToListAsync();

		return Ok(roles);
	}

	[HttpPost("roles")]
	public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto model)
	{
		var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
		if (roleExists)
			return BadRequest("Role already exists");

		var role = new IdentityRole(model.RoleName);
		var result = await _roleManager.CreateAsync(role);

		if (result.Succeeded)
			return Ok($"Role '{model.RoleName}' created successfully");

		return BadRequest(result.Errors);
	}

	// DTOs for Admin operations
	public class AssignRoleDto
	{
		public string RoleName { get; set; } = string.Empty;
	}

	public class CreateRoleDto
	{
		public string RoleName { get; set; } = string.Empty;
	}
}
