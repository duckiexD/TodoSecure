using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TodoSecure.Data;
using TodoSecure.Models;

namespace TodoSecure.Pages.Tasks;

[Authorize]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly IAuthorizationService _authz;

    public IndexModel(AppDbContext db, UserManager<AppUser> um, IAuthorizationService authz)
    {
        _db = db;
        _userManager = um;
        _authz = authz;
    }

    public List<TaskItem> Tasks { get; set; } = new();
    public HashSet<int> EditableIds { get; set; } = new();

    public async Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User)!;
        var isAdmin = User.IsInRole("Admin");

        var query = _db.Tasks.Include(t => t.User).AsQueryable();
        if (!isAdmin)
            query = query.Where(t => t.UserId == userId);

        Tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();

        foreach (var t in Tasks)
        {
            var res = await _authz.AuthorizeAsync(User, t, "CanEditTask");
            if (res.Succeeded) EditableIds.Add(t.Id);
        }
    }
}