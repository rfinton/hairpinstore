using Microsoft.AspNetCore.Identity;

namespace HairpinStore.Data;

public class RoleSeeder
{
	public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
	{
		var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
		var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
		var configuration = serviceProvider.GetRequiredService<IConfiguration>();

		// Define roles
		string[] roleNames = { "Administrator", "Customer", "Manager" };

		// Create roles if they don't exist
		foreach (var roleName in roleNames)
		{
			var roleExist = await roleManager.RoleExistsAsync(roleName);
			if (!roleExist)
			{
				var role = new IdentityRole(roleName);
				await roleManager.CreateAsync(role);
			}
		}

		// Create default admin user
		var adminEmail = configuration["AdminUser:Email"] ?? "admin@hairpinstore.com";
		var adminPassword = configuration["AdminUser:Password"] ?? "Admin123!";

		var adminUser = await userManager.FindByEmailAsync(adminEmail);
		if (adminUser == null)
		{
			adminUser = new IdentityUser
			{
				UserName = adminEmail,
				Email = adminEmail,
				EmailConfirmed = true
			};

			var result = await userManager.CreateAsync(adminUser, adminPassword);
			if (result.Succeeded)
			{
				await userManager.AddToRoleAsync(adminUser, "Administrator");
			}
		}
		else
		{
			if (!await userManager.IsInRoleAsync(adminUser, "Administrator"))
			{
				await userManager.AddToRoleAsync(adminUser, "Administrator");
			}
		}
	}
}
