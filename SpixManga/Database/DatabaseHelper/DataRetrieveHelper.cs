using SQLite;

namespace SpixManga.Database.DatabaseHelper;

internal class DataRetrieveHelper
{
    internal async Task<long> RetrieveSeriesNewId(SQLiteAsyncConnection connection, int seriesId)
    {
        string retrieveSeriesIdQuery = @"
            SELECT MuNewId FROM SeriesIdTable WHERE SeriesId = ?;";

        var seriesNewId = await connection.ExecuteScalarAsync<long>(retrieveSeriesIdQuery, seriesId);
        return seriesNewId;
    }

    internal async Task<int> RetrieveSeriesIdFromOld(SQLiteAsyncConnection connection, int muOldId)
    {
        string retrieveSeriesIdQuery = @"
            SELECT SeriesId FROM SeriesIdTable WHERE MuOldId = ?;";

        var seriesId = await connection.ExecuteScalarAsync<int>(retrieveSeriesIdQuery, muOldId);
        return seriesId;
    }

    internal async Task<int> RetrieveSeriesIdFromNew(SQLiteAsyncConnection connection, long muNewId)
    {
        string retrieveSeriesIdQuery = @"
            SELECT SeriesId FROM SeriesIdTable WHERE MuNewId = ?;";

        var seriesId = await connection.ExecuteScalarAsync<int>(retrieveSeriesIdQuery, muNewId);
        return seriesId;
    }

    internal async Task<HashSet<int>> RetrieveSeriesIdSet(SQLiteAsyncConnection connection, string siteSource)
    {
        string retrieveSeriesIdSetMuQuery = @"
            SELECT SeriesId FROM SeriesIdTable WHERE SiteSource = ?;";

        HashSet<int> seriesIdSet = new HashSet<int>(await connection.QueryScalarsAsync<int>(retrieveSeriesIdSetMuQuery, siteSource));
        return seriesIdSet;
    }

    internal async Task<HashSet<int>> RetrieveSeriesIdSet(SQLiteAsyncConnection connection, string siteSource, int startId, int endId)
    {
        string retrieveSeriesIdSetQuery = @"
            SELECT SeriesId FROM SeriesIdTable WHERE SiteSource = ? AND SeriesId BETWEEN ? AND ?;";

        HashSet<int> seriesIdSet = new HashSet<int>(await connection.QueryScalarsAsync<int>(retrieveSeriesIdSetQuery, siteSource, startId, endId));
        return seriesIdSet;
    }

    internal async Task<int> RetrieveGenreId(SQLiteAsyncConnection connection, string genreName)
    {
        string retrieveGenreIdQuery = @"
            SELECT GenreId FROM GenresTable WHERE GenreName = ?;";

        var genreId = await connection.ExecuteScalarAsync<int>(retrieveGenreIdQuery, genreName);
        return genreId;
    }

    internal async Task<int> RetrieveCategoryId(SQLiteAsyncConnection connection, string categoryName)
    {
        string retrieveCategoryIdQuery = @"
            SELECT CategoryId FROM CategoriesTable WHERE CategoryName = ?;";

        var categoryId = await connection.ExecuteScalarAsync<int>(retrieveCategoryIdQuery, categoryName);
        return categoryId;
    }
    
    internal async Task<int> RetrieveTagId(SQLiteAsyncConnection connection, string tagName)
    {
        string retrieveTagIdQuery = @"
            SELECT TagId FROM TagsTable WHERE TagName = ?;";

        var tagId = await connection.ExecuteScalarAsync<int>(retrieveTagIdQuery, tagName);
        return tagId;
    }
}
