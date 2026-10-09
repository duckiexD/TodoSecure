using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoSecure.Models;
using TodoSecure.Services;

namespace TodoSecure.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly SignInManager<AppUser> _sm;
    private readonly UserManager<AppUser> _um;
    private readonly JwtService _jwt;

    public AuthApiController(SignInManager<AppUser> sm, UserManager<AppUser> um, JwtService jwt)
    { _sm = sm; _um = um; _jwt = jwt; }

    public class LoginRequest
    {
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required] public string Password { get; set; } = "";
    }

    [HttpPost("login")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _um.FindByEmailAsync(req.Email);
        if (user == null) return Unauthorized();

        var result = await _sm.CheckPasswordSignInAsync(user, req.Password, lockoutOnFailure: true);
        if (!result.Succeeded) return Unauthorized();

        var token = await _jwt.CreateTokenAsync(user, _um);
        return Ok(new { token });
    }
}