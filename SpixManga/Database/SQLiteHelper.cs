using System.IO;

namespace SpixManga.Database;

internal class SQLiteHelper
{
    private const string DbNameMu = "MangaUpdate.sqlite";
    public string DbPathMu => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbNameMu);
}