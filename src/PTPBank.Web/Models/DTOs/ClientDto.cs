namespace PTPBank.Web.Models.DTOs;

public class ClientDto
{
    public int Code { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string IDNumber { get; set; } = string.Empty;
    public int AccountCount { get; set; }
}
