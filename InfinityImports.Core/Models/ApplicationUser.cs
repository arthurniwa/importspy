using Microsoft.AspNetCore.Identity;

namespace InfinityImports.Core.Models;

public class ApplicationUser : IdentityUser
{
    public string NomeCompleto { get; set; } = string.Empty;
}
