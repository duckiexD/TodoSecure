using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoSecure.Data;
using TodoSecure.Models;

namespace TodoSecure.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TasksApiController : ControllerBase
{
    private readonly AppDbContext _db;
    public TasksApiController(AppDbContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool IsAdmin => User.IsInRole("Admin");

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var q = _db.Tasks.AsQueryable();
        if (!IsAdmin) q = q.Where(t => t.UserId == UserId);
        return Ok(await q.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var t = await _db.Tasks.FindAsync(id);
        if (t == null) return NotFound();
        if (!IsAdmin && t.UserId != UserId) return Forbid();
        return Ok(t);
    }

    public class TaskDto
    {
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskDto dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = dto.IsCompleted,
            UserId = UserId   // берём из токена!
        };
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskDto dto)
    {
        var t = await _db.Tasks.FindAsync(id);
        if (t == null) return NotFound();
        if (!IsAdmin && t.UserId != UserId) return Forbid();

        t.Title = dto.Title;
        t.Description = dto.Description;
        t.IsCompleted = dto.IsCompleted;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _db.Tasks.FindAsync(id);
        if (t == null) return NotFound();
        if (!IsAdmin && t.UserId != UserId) return Forbid();

        _db.Tasks.Remove(t);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}