using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TodoSecure.Data;
using TodoSecure.Models;

namespace TodoSecure.Pages.Tasks;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IAuthorizationService _authz;

    public DeleteModel(AppDbContext db, IAuthorizationService authz)
    {
        _db = db;
        _authz = authz;
    }

    public TaskItem? Task { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Task = await _db.Tasks.FindAsync(id);
        if (Task == null) return NotFound();

        var res = await _authz.AuthorizeAsync(User, Task, "CanEditTask");
        if (!res.Succeeded) return Forbid();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        var res = await _authz.AuthorizeAsync(User, task, "CanEditTask");
        if (!res.Succeeded) return Forbid();

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}