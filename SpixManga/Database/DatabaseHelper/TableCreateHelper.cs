using SQLite;

namespace SpixManga.Database.DatabaseHelper;

internal class TableCreateHelper
{
    internal async Task CreateMangaIdTable(SQLiteAsyncConnection connection)
    {
        string createMangaIdTableQuery = @"
                CREATE TABLE IF NOT EXISTS MangaIdTable (
                    MangaId INTEGER PRIMARY KEY AUTOINCREMENT,
                    MuOldId TEXT NOT NULL UNIQUE,
                    MuNewId NOT NULL UNIQUE
                );";

        await connection.ExecuteAsync(createMangaIdTableQuery);
    }

    internal async Task CreateReadTable(SQLiteAsyncConnection connection)
    {
        string createReadTableQuery = @"
                CREATE TABLE IF NOT EXISTS ReadTable (
                    ReadId INTEGER PRIMARY KEY AUTOINCREMENT,
                    MangaId INTEGER NOT NULL,
                    Rating REAL CHECK (Rating >= 0 AND Rating <= 10),
                    ReadStatus TEXT NOT NULL CHECK (ReadStatus IN ('Reading', 'Completed', 'On Hold', 'Dropped', 'Plan to Read', 'Not Interested')) DEFAULT 'Reading',
                    Progress REAL NOT NULL CHECK (Progress >= 0),
                    Priority TEXT CHECK (Priority IN ('High', 'Medium', 'Low')),
                    Review TEXT,                    
                    StartDate DATETIME,
                    EndDate DATETIME,
                    LastUpdated DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (MangaId) REFERENCES MangaIdTable(MangaId) ON DELETE CASCADE
                );";

        await connection.ExecuteAsync(createReadTableQuery);
    }

    internal async Task CreateSessionTable(SQLiteAsyncConnection connection)
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