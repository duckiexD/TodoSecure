using Microsoft.AspNetCore.Identity;
using TodoSecure.Models;

namespace TodoSecure.Data;

public static class SeedData
{
    public const string AdminEmail = "admin@duck.com";
    public const string AdminPassword = "AdminDuck1e";

    public static async Task InitializeAsync(IServiceProvider sp)
    {
        Console.WriteLine("[SEED] Начало сидирования...");

        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = sp.GetRequiredService<UserManager<AppUser>>();

        // ---------- Роли ----------
        foreach (var role in new[] { "Admin", "Author", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var rr = await roleManager.CreateAsync(new IdentityRole(role));
                Console.WriteLine(rr.Succeeded
                    ? $"[SEED] Роль '{role}' создана"
                    : $"[SEED ERROR role] {role}: {string.Join("; ", rr.Errors.Select(e => e.Description))}");
            }
            else
            {
                Console.WriteLine($"[SEED] Роль '{role}' уже существует");
            }
        }

        // ---------- Админ ----------
        var admin = await userManager.FindByEmailAsync(AdminEmail);

        if (admin == null)
        {
            admin = new AppUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true,
                FullName = "Утиный Админ"
            };

            var result = await userManager.CreateAsync(admin, AdminPassword);

            if (result.Succeeded)
            {
                Console.WriteLine($"[SEED] Админ '{AdminEmail}' создан");

                var roleResult = await userManager.AddToRoleAsync(admin, "Admin");
                if (roleResult.Succeeded)
                    Console.WriteLine($"[SEED] Роль 'Admin' назначена {AdminEmail}");
                else
                    Console.WriteLine("[SEED ERROR role assign] " +
                        string.Join("; ", roleResult.Errors.Select(e => e.Description)));
            }
            else
            {
                Console.WriteLine("[SEED ERROR user] " +
                    string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            Console.WriteLine($"[SEED] Админ '{AdminEmail}' уже существует (Id={admin.Id})");

            // Если роль вдруг потерялась — восстановим
            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                Console.WriteLine($"[SEED] Роль 'Admin' переприсвоена {AdminEmail}");
            }
        }

        Console.WriteLine("[SEED] Сидирование завершено");
    }
}