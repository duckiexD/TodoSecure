using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TodoSecure.Data;

namespace TodoSecure.Pages.Tasks;

[Authorize]
public class MarkDoneModel : PageModel
{
    private readonly AppDbContext _db;
    public MarkDoneModel(AppDbContext db) => _db = db;

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!User.IsInRole("Admin") && task.UserId != userId)
            return Forbid();

        task.IsCompleted = true;
        await _db.SaveChangesAsync();
        return new JsonResult(new { ok = true });
    }
}