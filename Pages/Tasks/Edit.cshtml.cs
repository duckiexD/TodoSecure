using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TodoSecure.Data;
using TodoSecure.Models;

namespace TodoSecure.Pages.Tasks;

[Authorize]
public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IAuthorizationService _authz;

    public EditModel(AppDbContext db, IAuthorizationService authz)
    {
        _db = db;
        _authz = authz;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Заголовок обязателен")]
        [StringLength(200)]
        public string Title { get; set; } = "";

        [StringLength(2000)]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        var res = await _authz.AuthorizeAsync(User, task, "CanEditTask");
        if (!res.Succeeded) return Forbid();

        Input = new InputModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var task = await _db.Tasks.FindAsync(Input.Id);
        if (task == null) return NotFound();

        var res = await _authz.AuthorizeAsync(User, task, "CanEditTask");
        if (!res.Succeeded) return Forbid();

        if (!ModelState.IsValid) return Page();

        task.Title = Input.Title;
        task.Description = Input.Description;
        task.IsCompleted = Input.IsCompleted;
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}