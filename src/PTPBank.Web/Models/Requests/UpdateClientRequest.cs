namespace PTPBank.Web.Models.Requests;

public class UpdateClientRequest
{
    public int Code { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string IDNumber { get; set; } = string.Empty;
}
