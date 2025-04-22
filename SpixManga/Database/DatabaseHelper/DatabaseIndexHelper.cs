using SQLite;

namespace SpixManga.Database.DatabaseHelper;

    internal class DatabaseIndexHelper
    {
        // TODO: ReadTable, SessionTable Index

        internal async Task CreateIndex(SQLiteAsyncConnection connection)
        {
            await connection.ExecuteAsync(indexSeriesDetailsSeriesTypeQuery);
            await connection.ExecuteAsync(indexSeriesDetailsYearQuery);
            await connection.ExecuteAsync(indexSeriesDetailsBayesianRatingQuery);
            await connection.ExecuteAsync(indexSeriesDetailsOriginCompletedQuery);
            await connection.ExecuteAsync(indexSeriesDetailsLicensedQuery);
            await connection.ExecuteAsync(indexSeriesDetailsScanlationCompletedQuery);
            await connection.ExecuteAsync(indexSeriesDetailsLastUpdatedQuery);

            await connection.ExecuteAsync(indexSeriesGenresSeriesIdQuery);
            await connection.ExecuteAsync(indexSeriesGenresGenreIdQuery);

            await connection.ExecuteAsync(indexSeriesCategoriesSeriesIdQuery);
            await connection.ExecuteAsync(indexSeriesCategoriesCategoryIdQuery);

            await connection.ExecuteAsync(indexSeriesSynonymsSeriesIdQuery);
        }


        // Individual Search
        // SeriesTable
        // Skip - Description, ImageUrl, LatestChapter (No valid/consistent value), OriginStatus (Same)
        // AnimeStart, AnimeEnd
        private readonly string indexSeriesDetailsSeriesTypeQuery = @"
            CREATE INDEX idx_SeriesTable_SeriesType ON SeriesDetailsTable (SeriesType);";
        private readonly string indexSeriesDetailsYearQuery = @"
            CREATE INDEX idx_SeriesDetails_Year ON SeriesDetailsTable (Year);";
        private readonly string indexSeriesDetailsBayesianRatingQuery = @"
            CREATE INDEX idx_SeriesDetails_BayesianRating ON SeriesDetailsTable (BayesianRating);";
        private readonly string indexSeriesDetailsOriginCompletedQuery = @"
            CREATE INDEX idx_SeriesDetails_OriginCompleted ON SeriesDetailsTable (OriginCompleted);";
        private readonly string indexSeriesDetailsLicensedQuery = @"
            CREATE INDEX idx_SeriesDetails_Licensed ON SeriesDetailsTable (Licensed);";
        private readonly string indexSeriesDetailsScanlationCompletedQuery = @"
            CREATE INDEX idx_SeriesDetails_ScanlationCompleted ON SeriesDetailsTable (ScanlationCompleted);";
        private readonly string indexSeriesDetailsLastUpdatedQuery = @"
            CREATE INDEX idx_SeriesDetails_LastUpdated ON SeriesDetailsTable (LastUpdated);";

        //Unique column have auto index -
        //      GenreName, ThemeName

        //SeriesGenresTable - for single column query
        private readonly string indexSeriesGenresSeriesIdQuery = @"
            CREATE INDEX idx_SeriesGenres_SeriesId ON SeriesGenresTable (SeriesId);";
        private readonly string indexSeriesGenresGenreIdQuery = @"
            CREATE INDEX idx_SeriesGenres_GenreId ON SeriesGenresTable (GenreId);";

        //SeriesCategoriesTable
        private readonly string indexSeriesCategoriesSeriesIdQuery = @"
            CREATE INDEX idx_SeriesCategories_SeriesId ON SeriesCategoriesTable (SeriesId);";
        private readonly string indexSeriesCategoriesCategoryIdQuery = @"
            CREATE INDEX idx_SeriesCategories_CategoryId ON SeriesCategoriesTable (CategoryId);";

        //SeriesSynonymsTable
        private readonly string indexSeriesSynonymsSeriesIdQuery = @"
            CREATE INDEX idx_SeriesSynonyms_SeriesId ON SeriesSynonymsTable (SeriesId);";


        // TODO: Highest Composite Search
        //private readonly string indexAnimeCombinedQuery = @"
        //    CREATE INDEX idx_Anime_Combined ON SeriesTable (
        //        MalId, AnimeType, Source, AirStatus, Rating, 
        //        Season, Year, AirStart, AirEnd
        //    );";
        //private readonly string indexAnimeFullCombinedQuery = @"
        //    CREATE INDEX idx_Anime_Combined ON SeriesTable (
        //        MalId, AnimeType, Source, Episodes, AirStatus, Duration,
        //        Rating, Season, Year, AirStart, AirEnd
        //    );";
    }
