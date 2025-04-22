using System.IO;

namespace SpixManga.Database;

internal class SQLiteHelper
{
    private const string DbNameSeriesMu = "SeriesMuDb.sqlite";
    public string DbPathSeriesMu => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbNameSeriesMu);

    private const string DbNameSeriesMuImage = "SeriesMuImageDb.sqlite";
    public string DbPathSeriesMuImage => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbNameSeriesMuImage);
}