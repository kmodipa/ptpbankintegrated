namespace PTPBank.Domain.Common;

public abstract class EntityBase
{
    public int Code { get; set; }
    public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedDateUtc { get; set; } = DateTime.UtcNow;
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
