using System.ComponentModel.DataAnnotations;

namespace PTPBank.Web.Models.ViewModels;

public class ClientFormViewModel
{
    public int? Code { get; set; }

    [Display(Name = "Name")]
    public string? Name { get; set; }

    [Display(Name = "Surname")]
    public string? Surname { get; set; }

    [Required, MaxLength(50), Display(Name = "ID Number")]
    public string IDNumber { get; set; } = string.Empty;
}
