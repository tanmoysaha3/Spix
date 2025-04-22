using SQLite;

namespace SpixManga.Database.DatabaseHelper;

    internal class DataRetrieveHelper
    {
        // Dynamic Single Value Query Function - Extra Complex
        //internal async Task<T> RetrieveSingleValue<T>(SQLiteAsyncConnection connection, string inputFieldName,
        //    object inputFieldValue, string outputFieldName, string tableName)
        //{
        //    string retrieveSeriesIdQuery = $@"
        //        SELECT {outputFieldName} FROM {tableName} WHERE {inputFieldName} = {inputFieldValue};";

        //    var seriesId = await connection.ExecuteScalarAsync<T>(retrieveSeriesIdQuery);
        //    return seriesId;
        //}

        internal async Task<int> RetrieveSeriesIdOld(SQLiteAsyncConnection connection, int muOldId)
        {
            string retrieveSeriesIdQuery = @"
                SELECT SeriesId FROM SeriesIdTable WHERE MuOldId = ?;";

            var seriesId = await connection.ExecuteScalarAsync<int>(retrieveSeriesIdQuery, muOldId);
            return seriesId;
        }

        internal async Task<int> RetrieveSeriesIdNew(SQLiteAsyncConnection connection, long muNewId)
        {
            string retrieveSeriesIdQuery = @"
                SELECT SeriesId FROM SeriesIdTable WHERE MuNewId = ?;";

            var seriesId = await connection.ExecuteScalarAsync<int>(retrieveSeriesIdQuery, muNewId);
            return seriesId;
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
    }
