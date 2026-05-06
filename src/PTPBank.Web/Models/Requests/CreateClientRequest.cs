namespace PTPBank.Web.Models.Requests;

public class CreateClientRequest
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string IDNumber { get; set; } = string.Empty;
}
