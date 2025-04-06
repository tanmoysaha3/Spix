namespace SpixManga.Database.Models;

public class SessionTableCfModel
{
    public int SessionId { get; set; }
    public int ReadId { get; set; }
    public double? Chapter { get; set; }
    public string? Comment { get; set; }
    public string? LastUpdated { get; set; }
}