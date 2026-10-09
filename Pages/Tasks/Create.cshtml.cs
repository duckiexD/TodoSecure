using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TodoSecure.Data;
using TodoSecure.Models;

namespace TodoSecure.Pages.Tasks;

[Authorize]
public class CreateModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public CreateModel(AppDbContext db, UserManager<AppUser> um)
    {
        _db = db;
        _userManager = um;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Заголовок обязателен")]
        [StringLength(200)]
        public string Title { get; set; } = "";

        [StringLength(2000)]
        public string? Description { get; set; }
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        _db.Tasks.Add(new TaskItem
        {
            Title = Input.Title,
            Description = Input.Description,
            UserId = _userManager.GetUserId(User)!
        });
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}