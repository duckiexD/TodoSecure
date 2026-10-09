using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TodoSecure.Models;

namespace TodoSecure.Pages.Admin;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    public IndexModel(UserManager<AppUser> um) => _userManager = um;

    public List<(AppUser User, IList<string> Roles)> Users { get; set; } = new();

    public async Task OnGetAsync()
    {
        var all = await _userManager.Users.ToListAsync();
        foreach (var u in all)
        {
            var roles = await _userManager.GetRolesAsync(u);
            Users.Add((u, roles));
        }
    }
}