using SQLite;

namespace SpixManga.Database.DatabaseHelper;

    internal class TableCreateHelper
    {
        internal async Task InitializeDatabase(SQLiteAsyncConnection connection)
        {
            FTS_Helper _fts_Helper = new FTS_Helper();
            DatabaseIndexHelper _databaseIndexHelper = new DatabaseIndexHelper();

            _ = connection.ExecuteAsync("PRAGMA foreign_keys = ON;");
            await CreateSeriesIdTable(connection); 
            await CreateSeriesDetailsTable(connection);
            await CreateGenresTable(connection);
            await CreateSeriesGenresTable(connection);
            await CreateCategoriesTable(connection);
            await CreateSeriesCategoriesTable(connection);
            await CreateSeriesRelationsTable(connection);
            await CreateSeriesSynonymsTable(connection);
            await CreateReadTable(connection);
            await CreateSessionTable(connection);
            await _databaseIndexHelper.CreateIndex(connection);
            await _fts_Helper.CreateSeriesDetailsFTSTable(connection);
            await _fts_Helper.CreateSeriesSynonymsFTSTable(connection);
        }

        private async Task CreateSeriesIdTable(SQLiteAsyncConnection connection)
        {
            string createSeriesIdTableQuery = @"
                CREATE TABLE IF NOT EXISTS SeriesIdTable (
                    SeriesId INTEGER PRIMARY KEY AUTOINCREMENT,
                    MuOldId TEXT NOT NULL UNIQUE,
                    MuNewId NOT NULL UNIQUE
                );";

            await connection.ExecuteAsync(createSeriesIdTableQuery);
        }

        private async Task CreateSeriesDetailsTable(SQLiteAsyncConnection connection)
        {
            // OriginCompleted manual field based on "OriginStatus"
            string createSeriesDetailsTableQuery = @"
                CREATE TABLE IF NOT EXISTS SeriesDetailsTable (
                    SeriesId INTEGER PRIMARY KEY,
                    SeriesTitle TEXT NOT NULL,
                    Description TEXT,
                    SeriesType TEXT,
                    Year INTEGER,
                    BayesianRating INTEGER,
                    ImageUrl TEXT,
                    LatestChapter INTEGER,
                    OriginStatus TEXT,
                    OriginCompleted INTEGER,
                    Licensed INTEGER,
                    ScanlationCompleted INTEGER,
                    AnimeStart TEXT,
                    AnimeEnd TEXT,
                    LastUpdated DATETIME,
                    FOREIGN KEY (SeriesId) REFERENCES SeriesIdTable(SeriesId) ON DELETE CASCADE
                );";
            await connection.ExecuteAsync(createSeriesDetailsTableQuery);
        }

        private async Task CreateGenresTable(SQLiteAsyncConnection connection)
        {
            string createGenresTableQuery = @"
                CREATE TABLE IF NOT EXISTS GenresTable (
                    GenreId INTEGER PRIMARY KEY AUTOINCREMENT,
                    GenreName TEXT NOT NULL UNIQUE
                );";

            await connection.ExecuteAsync(createGenresTableQuery);
        }

        private async Task CreateSeriesGenresTable(SQLiteAsyncConnection connection)
        {
            string createSeriesGenresTableQuery = @"
                CREATE TABLE IF NOT EXISTS SeriesGenresTable (
                    SeriesId INTEGER NOT NULL,
                    GenreId INTEGER NOT NULL,
                    PRIMARY KEY (SeriesId, GenreId),
                    FOREIGN KEY (SeriesId) REFERENCES SeriesIdTable(SeriesId) ON DELETE CASCADE,
                    FOREIGN KEY (GenreId) REFERENCES GenresTable(GenreId) ON DELETE CASCADE
                );";

            await connection.ExecuteAsync(createSeriesGenresTableQuery);
        }

        private async Task CreateCategoriesTable(SQLiteAsyncConnection connection)
        {
            string createCategoriesTableQuery = @"
                CREATE TABLE IF NOT EXISTS CategoriesTable (
                    CategoryId INTEGER PRIMARY KEY AUTOINCREMENT,
                    CategoryName TEXT NOT NULL UNIQUE
                );";

            await connection.ExecuteAsync(createCategoriesTableQuery);
        }

        private async Task CreateSeriesCategoriesTable(SQLiteAsyncConnection connection)
        {
            string createSeriesCategoriesTableQuery = @"
                CREATE TABLE IF NOT EXISTS SeriesCategoriesTable (
                    SeriesId INTEGER NOT NULL,
                    CategoryId INTEGER NOT NULL,
                    PRIMARY KEY (SeriesId, CategoryId),
                    FOREIGN KEY (SeriesId) REFERENCES SeriesIdTable(SeriesId) ON DELETE CASCADE,
                    FOREIGN KEY (CategoryId) REFERENCES CategoriesTable(CategoryId) ON DELETE CASCADE
                );";

            await connection.ExecuteAsync(createSeriesCategoriesTableQuery);
        }

        private async Task CreateSeriesRelationsTable(SQLiteAsyncConnection connection)
        {
            string createSeriesRelationsTableQuery = @"
                CREATE TABLE IF NOT EXISTS SeriesRelationsTable (
                    RelationId INTEGER PRIMARY KEY AUTOINCREMENT,
                    SeriesId INTEGER NOT NULL,
                    RelationType TEXT NOT NULL,
                    RelatedSeriesId INTEGER NOT NULL,
                    RelatedSeriesName TEXT NOT NULL,
                    UNIQUE (SeriesId, RelationType, RelatedSeriesId),
                    FOREIGN KEY (SeriesId) REFERENCES SeriesIdTable(SeriesId) ON DELETE CASCADE
                    FOREIGN KEY (RelatedSeriesId) REFERENCES SeriesIdTable(SeriesId) ON DELETE CASCADE
                );";

            await connection.ExecuteAsync(createSeriesRelationsTableQuery);
        }

        private async Task CreateSeriesSynonymsTable(SQLiteAsyncConnection connection)
        {
            string createSeriesSynonymsTableQuery = @"
                CREATE TABLE IF NOT EXISTS SeriesSynonymsTable (
                    SynonymId INTEGER PRIMARY KEY AUTOINCREMENT,
                    SeriesId INTEGER NOT NULL,                    
                    Synonym TEXT NOT NULL,
                    UNIQUE (SeriesId, Synonym),
                    FOREIGN KEY (SeriesId) REFERENCES SeriesIdTable(SeriesId) ON DELETE CASCADE
                );";

            await connection.ExecuteAsync(createSeriesSynonymsTableQuery);
        }

        private async Task CreateReadTable(SQLiteAsyncConnection connection)
        {
            // TODO: Add SiteName, SiteUrl
            string createReadTableQuery = @"
                CREATE TABLE IF NOT EXISTS ReadTable (
                    ReadId INTEGER PRIMARY KEY AUTOINCREMENT,
                    SeriesId INTEGER NOT NULL,
                    Rating REAL CHECK (Rating >= 0 AND Rating <= 10),
                    ReadStatus TEXT NOT NULL CHECK (ReadStatus IN ('Reading', 'Completed', 'On Hold', 'Dropped', 'Plan to Read', 'Not Interested')) DEFAULT 'Reading',
                    Progress REAL NOT NULL CHECK (Progress >= 0),
                    Priority TEXT CHECK (Priority IN ('High', 'Medium', 'Low')),
                    Review TEXT,                    
                    StartDate DATETIME,
                    EndDate DATETIME,
                    LastUpdated DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (SeriesId) REFERENCES SeriesIdTable(SeriesId) ON DELETE CASCADE
                );";

            await connection.ExecuteAsync(createReadTableQuery);
        }

        private async Task CreateSessionTable(SQLiteAsyncConnection connection)
        {
            string createSessionTableQuery = @"
                CREATE TABLE IF NOT EXISTS SessionTable (
                    SessionId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReadId INTEGER NOT NULL,
                    Chapter REAL NOT NULL,
                    Comment TEXT NOT NULL,
                    LastUpdated DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (ReadId) REFERENCES ReadTable(ReadId) ON DELETE CASCADE
                );";

            await connection.ExecuteAsync(createSessionTableQuery);
        }

    }
