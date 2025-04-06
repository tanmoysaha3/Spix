using SpixManga.Database.Models;
using SQLite;

namespace SpixManga.Database.DatabaseHelper;

internal class DataUpdateHelper
{
    internal async Task updateReadTable(SQLiteAsyncConnection connection, ReadTableCfModel readTableCfModel)
    {
        string updateReadQuery = @"
                UPDATE ReadTable 
                SET 
                    MangaId = ?, 
                    Rating = ?, 
                    ReadStatus = ?, 
                    Progress = ?, 
                    Priority = ?, 
                    Review = ?, 
                    StartDate = ?, 
                    EndDate = ?, 
                    LastUpdated = ?
                WHERE ReadId = ?
                "
            ;

        await connection.ExecuteAsync(updateReadQuery, readTableCfModel.MangaId, readTableCfModel.Rating,
            readTableCfModel.ReadStatus, readTableCfModel.Progress, readTableCfModel.Priority,
            readTableCfModel.Review, readTableCfModel.StartDate, readTableCfModel.EndDate, readTableCfModel.LastUpdated, readTableCfModel.ReadId);
    }
}