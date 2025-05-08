using SQLite;

namespace SpixManga.Database.DatabaseHelper;

internal class FTS_Helper
{
    //Full-Text-Search (FTS)
    internal async Task CreateSeriesDetailsFTSTable(SQLiteAsyncConnection connection)
    {
        string CreateSeriesDetailsFTSTableQuery = @"
            CREATE VIRTUAL TABLE IF NOT EXISTS SeriesDetails_FTS USING fts5(
                SeriesId, 
                SeriesTitle, 
                content='SeriesDetailsTable',
                content_rowid='SeriesId'
            );";
        await connection.ExecuteAsync(CreateSeriesDetailsFTSTableQuery);

        string insertTriggerQuery = @"
            CREATE TRIGGER IF NOT EXISTS InsertSeriesDetails_FTS
            AFTER INSERT ON SeriesDetailsTable
            BEGIN
                INSERT INTO SeriesDetails_FTS(SeriesId, SeriesTitle)
                VALUES (new.SeriesId, new.SeriesTitle);
            END;";
        await connection.ExecuteAsync(insertTriggerQuery);

        string updateTriggerQuery = @"
            CREATE TRIGGER IF NOT EXISTS UpdateSeriesDetails_FTS
            AFTER UPDATE ON SeriesDetailsTable
            BEGIN
                UPDATE SeriesDetails_FTS
                SET SeriesTitle = new.SeriesTitle
                WHERE SeriesId = new.SeriesId;
            END;";
        await connection.ExecuteAsync(updateTriggerQuery);

        string deleteTriggerQuery = @"
            CREATE TRIGGER IF NOT EXISTS DeleteSeriesDetails_FTS
            AFTER DELETE ON SeriesDetailsTable
            BEGIN
                DELETE FROM SeriesDetails_FTS
                WHERE SeriesId = old.SeriesId;
            END;";
        await connection.ExecuteAsync(deleteTriggerQuery);
    }

    internal async Task CreateSeriesSynonymsFTSTable(SQLiteAsyncConnection connection)
    {
        string CreateSeriesSynonymsFTSTableQuery = @"
            CREATE VIRTUAL TABLE IF NOT EXISTS SeriesSynonyms_FTS USING fts5(
                SeriesId,
                Synonym,
                content='SeriesSynonymsTable',
                content_rowid='SynonymId'
            );";
        await connection.ExecuteAsync(CreateSeriesSynonymsFTSTableQuery);

        string insertTriggerQuery = @"
            CREATE TRIGGER IF NOT EXISTS InsertSeriesSynonyms_FTS
            AFTER INSERT ON SeriesSynonymsTable
            BEGIN
                INSERT INTO SeriesSynonyms_FTS(SeriesId, Synonym)
                VALUES (new.SeriesId, new.Synonym);
            END;";
        await connection.ExecuteAsync(insertTriggerQuery);

        string updateTriggerQuery = @"
            CREATE TRIGGER IF NOT EXISTS UpdateSeriesSynonyms_FTS
            AFTER UPDATE ON SeriesSynonymsTable
            BEGIN
                UPDATE SeriesSynonyms_FTS
                SET Synonym = new.Synonym
                WHERE SeriesId = new.SeriesId;
            END;";
        await connection.ExecuteAsync(updateTriggerQuery);

        string deleteTriggerQuery = @"
            CREATE TRIGGER IF NOT EXISTS DeleteSeriesSynonyms_FTS
            AFTER DELETE ON SeriesSynonymsTable
            BEGIN
                DELETE FROM SeriesSynonyms_FTS
                WHERE SeriesId = old.SeriesId;
            END;";
        await connection.ExecuteAsync(deleteTriggerQuery);
    }
}
