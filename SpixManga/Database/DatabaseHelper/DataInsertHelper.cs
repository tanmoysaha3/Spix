using SpixManga.Database.RetrieveModel;
using SQLite;

namespace SpixManga.Database.DatabaseHelper;

    internal class DataInsertHelper
    {
        internal async Task insertSeriesId(SQLiteAsyncConnection connection, SeriesIdModel seriesIdModel)
        {
            string insertSeriesIdQuery = @"
                INSERT OR IGNORE INTO SeriesIdTable (
                    MuOldId, MuNewId
                )
                VALUES (
                    ?, ?
                );";

            await connection.ExecuteAsync(insertSeriesIdQuery, seriesIdModel.MuOldId, seriesIdModel.MuNewId);
        }

        internal async Task InsertSeriesDetails(SQLiteAsyncConnection connection, SeriesDetailsModel seriesDetailsModel)
        {
            string insertSeriesDetailsQuery = @"
                INSERT OR IGNORE INTO SeriesDetailsTable (
                    SeriesId, SeriesTitle, Description, SeriesType, Year,
                    BayesianRating, ImageUrl, LatestChapter, OriginStatus, OriginCompleted,
                    Licensed, ScanlationCompleted, AnimeStart, AnimeEnd, LastUpdated
                )
                VALUES (
                    ?, ?, ?, ?, ?,
                    ?, ?, ?, ?, ?,
                    ?, ?, ?, ?, ?
                );";

            await connection.ExecuteAsync(insertSeriesDetailsQuery, seriesDetailsModel.SeriesId, seriesDetailsModel.SeriesTitle, 
                seriesDetailsModel.Description, seriesDetailsModel.SeriesType, seriesDetailsModel.Year, 
                seriesDetailsModel.BayesianRating, seriesDetailsModel.ImageUrl, seriesDetailsModel.LatestChapter, 
                seriesDetailsModel.OriginStatus, seriesDetailsModel.OriginCompleted, seriesDetailsModel.Licensed, 
                seriesDetailsModel.ScanlationCompleted, seriesDetailsModel.AnimeStart, seriesDetailsModel.AnimeEnd, 
                seriesDetailsModel.LastUpdated);
        }

        internal async Task InsertGenres(SQLiteAsyncConnection connection, string genreName)
        {
            string insertGenresQuery = @"
                INSERT OR IGNORE INTO GenresTable (
                    GenreName
                )
                VALUES (
                    ?
                );";
            await connection.ExecuteAsync(insertGenresQuery, genreName);
        }

        internal async Task InsertSeriesGenres(SQLiteAsyncConnection connection, int seriesId, int genreId)
        {
            string insertSeriesGenresQuery = @"
                INSERT OR IGNORE INTO SeriesGenresTable (
                    SeriesId, GenreId
                )
                VALUES (
                    ?, ?
                );";
            await connection.ExecuteAsync(insertSeriesGenresQuery, seriesId, genreId);
        }

        internal async Task InsertCategories(SQLiteAsyncConnection connection, string categoryName)
        {
            string insertCategoriesQuery = @"
                INSERT OR IGNORE INTO CategoriesTable (
                    CategoryName
                )
                VALUES (
                    ?
                );";
            await connection.ExecuteAsync(insertCategoriesQuery, categoryName);
        }

        internal async Task InsertSeriesCategories(SQLiteAsyncConnection connection, int seriesId, int categoryId)
        {
            string insertSeriesCategoriesQuery = @"
                INSERT OR IGNORE INTO SeriesCategoriesTable (
                    SeriesId, CategoryId
                )
                VALUES (
                    ?, ?
                );";
            await connection.ExecuteAsync(insertSeriesCategoriesQuery, seriesId, categoryId);
        }

        internal async Task InsertSeriesRelations(SQLiteAsyncConnection connection, SeriesRelationsModel seriesRelationsModel)
        {
            string insertSeriesRelationsQuery = @"
                INSERT OR IGNORE INTO SeriesRelationsTable (
                    SeriesId, RelationType, RelatedSeriesId, RelatedSeriesName
                )
                VALUES (
                    ?, ?, ?, ?
                );";
            await connection.ExecuteAsync(insertSeriesRelationsQuery, seriesRelationsModel.SeriesId, seriesRelationsModel.RelationType,
                seriesRelationsModel.RelatedSeriesId, seriesRelationsModel.RelatedSeriesName);
        }

        internal async Task InsertSeriesSynonyms(SQLiteAsyncConnection connection, int seriesId, string synonym)
        {
            string insertSeriesSynonymsQuery = @"
                INSERT OR IGNORE INTO SeriesSynonymsTable (
                    SeriesId, Synonym
                )
                VALUES (
                    ?, ?
                );";

            await connection.ExecuteAsync(insertSeriesSynonymsQuery, seriesId, synonym);
        }

        internal async Task insertRead(SQLiteAsyncConnection connection, ReadModel readModel)
        {
            string insertReadQuery = @"
                INSERT OR IGNORE INTO ReadTable (
                    SeriesId, Rating, ReadStatus, Progress, Priority,
                    Review, StartDate, EndDate, LastUpdated
                )
                VALUES (
                    ?, ?, ?, ?, ?,
                    ?, ?, ?, ?
                );";

            await connection.ExecuteAsync(insertReadQuery, readModel.SeriesId, readModel.Rating,
                readModel.ReadStatus, readModel.Progress, readModel.Priority,
                readModel.Review, readModel.StartDate, readModel.EndDate, readModel.LastUpdated);
        }

        internal async Task insertSession(SQLiteAsyncConnection connection, SessionModel sessionModel)
        {
            string insertSessionQuery = @"
                INSERT OR IGNORE INTO SessionTable ( 
                    ReadId, Chapter, Comment, LastUpdated
                )
                VALUES (
                    ?, ?, ?, ?
                );";

            await connection.ExecuteAsync(insertSessionQuery, sessionModel.ReadId, sessionModel.Chapter,
                sessionModel.Comment, sessionModel.LastUpdated);
        }
    }
