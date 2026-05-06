using Microsoft.AspNetCore.Identity;

namespace PTPBank.Web.Data.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}
