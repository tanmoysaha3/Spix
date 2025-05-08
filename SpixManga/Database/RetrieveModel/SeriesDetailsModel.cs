namespace SpixManga.Database.RetrieveModel;

public class SeriesDetailsModel
{
    public int SeriesId { get; set; }
    public string? SeriesTitle { get; set; }
    public string? Description { get; set; }
    public string? SeriesType { get; set; }
    public int? Year { get; set; }
    public double? Rating { get; set; }
    public string? ImageUrl { get; set; }
    public int? LatestChapter { get; set; }
    public string? OriginStatus { get; set; }
    public int? OriginCompleted { get; set; }
    public int? Licensed { get; set; }
    public int? ScanlationCompleted { get; set; }
    public string? AnimeStart { get; set; }
    public string? AnimeEnd { get; set; }
    public string? LastUpdated { get; set; }
}
