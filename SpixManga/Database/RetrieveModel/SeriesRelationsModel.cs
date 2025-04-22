namespace SpixManga.Database.RetrieveModel;

public class SeriesRelationsModel
{
    public int? RelationId { get; set; }
    public int? SeriesId { get; set; }
    public string? RelationType { get; set; }
    public int? RelatedSeriesId { get; set; }
    public string? RelatedSeriesName { get; set; }
}
