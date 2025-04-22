namespace SpixManga.Database.RetrieveModel;

public class ReadModel
{
    public int ReadId { get; set; }
    public int SeriesId { get; set; }
    public double? Rating { get; set; }
    public string? ReadStatus { get; set; }
    public double? Progress { get; set; }
    public string? Priority { get; set; }
    public string? Review { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? LastUpdated { get; set; }
}
