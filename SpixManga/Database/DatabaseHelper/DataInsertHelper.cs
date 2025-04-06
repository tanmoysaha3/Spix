using SpixManga.Database.Models;
using SQLite;

namespace SpixManga.Database.DatabaseHelper;

internal class DataInsertHelper
{
    internal async Task insertMangaId(SQLiteAsyncConnection connection, string MuOldId, long MuNewId)
    {
        string insertMangaIdQuery = @"
                INSERT OR IGNORE INTO MangaIdTable (
                    MuOldId, MuNewId
                )
                VALUES (
                    ?, ?
                );";

        await connection.ExecuteAsync(insertMangaIdQuery, MuOldId, MuNewId);
    }

    internal async Task insertRead(SQLiteAsyncConnection connection, ReadTableCfModel readTableCfModel)
    {
        string insertReadQuery = @"
                INSERT OR IGNORE INTO ReadTable (
                    MangaId, Rating, ReadStatus, Progress, Priority,
                    Review, StartDate, EndDate, LastUpdated
                )
                VALUES (
                    ?, ?, ?, ?, ?,
                    ?, ?, ?, ?
                );";

        await connection.ExecuteAsync(insertReadQuery, readTableCfModel.MangaId, readTableCfModel.Rating,
            readTableCfModel.ReadStatus, readTableCfModel.Progress, readTableCfModel.Priority,
            readTableCfModel.Review, readTableCfModel.StartDate, readTableCfModel.EndDate,
            readTableCfModel.LastUpdated);
    }

    internal async Task insertSession(SQLiteAsyncConnection connection, SessionTableCfModel sessionTableCfModel)
    {
        string insertSessionQuery = @"
                INSERT OR IGNORE INTO SessionTable ( 
                    ReadId, Chapter, Comment, LastUpdated
                )
                VALUES (
                    ?, ?, ?, ?
                );";

        await connection.ExecuteAsync(insertSessionQuery, sessionTableCfModel.ReadId, sessionTableCfModel.Chapter,
            sessionTableCfModel.Comment, sessionTableCfModel.LastUpdated);
    }
}