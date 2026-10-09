using Microsoft.AspNetCore.Authorization;

namespace TodoSecure.Authorization;

public class CanEditTaskRequirement : IAuthorizationRequirement { }