using System.Net.Http;
using Newtonsoft.Json.Linq;
using SpixManga.Database.DatabaseHelper;
using SpixManga.Database.RetrieveModel;
using SpixManga.Misc;
using SQLite;

namespace SpixManga.GetData;

internal class ApiSeriesFull
{
    DataInsertHelper _dataInsertHelper = new();
    DataRetrieveHelper _dataRetrieveHelper = new();
    SeriesDetailsModel _seriesDetailsModel = new();
    SeriesRelationsModel _seriesRelationsModel = new();

    private async Task<Dictionary<long,int>> RetrieveSeriesNewIdList(SQLiteAsyncConnection connection, int startId, int endId)
    {
        HashSet<int> seriesIdSet = await _dataRetrieveHelper.RetrieveSeriesIdSet(connection, "MangaUpdates", startId, endId);
        Dictionary<long, int> seriesNewIdDict = new();

        foreach (var seriesId in seriesIdSet)
        {
            long? seriesNewId = await _dataRetrieveHelper.RetrieveSeriesNewId(connection, seriesId);
            seriesNewIdDict[seriesNewId.Value] = seriesId;
        }

        return seriesNewIdDict;
    }

    internal async Task GetSeriesFull(SQLiteAsyncConnection connection, HttpClient httpClient, int startId, int endId)
    {
        Dictionary<long, int> seriesNewIdDict = await RetrieveSeriesNewIdList(connection, startId, endId);
        foreach (var (seriesNewId, seriesId) in seriesNewIdDict)
        {
            await InternetHelper.WaitForInternetAsync();
            string seriesUrl = $"https://api.mangaupdates.com/v1/series/{seriesNewId}";
            HttpResponseMessage seriesResponse = await httpClient.GetAsync(seriesUrl);
            var seriesContent = await seriesResponse.Content.ReadAsStringAsync();
            JToken seriesJson = JObject.Parse(seriesContent);

            _seriesDetailsModel.SeriesId = seriesId;
            _seriesDetailsModel.SeriesTitle = (string)seriesJson["title"]!;
            _seriesDetailsModel.Description = string.IsNullOrWhiteSpace(seriesJson["description"]?.ToString())
                                            ? (string?)null : seriesJson["description"]?.Value<string?>();
            _seriesDetailsModel.SeriesType = string.IsNullOrWhiteSpace(seriesJson["type"]?.ToString())
                                            ? (string?)null : seriesJson["type"]?.Value<string?>();
            _seriesDetailsModel.Year = string.IsNullOrWhiteSpace(seriesJson["year"]?.ToString())
                                            ? (int?)null : seriesJson["year"]?.Value<int?>();
            _seriesDetailsModel.Rating = string.IsNullOrWhiteSpace(seriesJson["bayesian_rating"]?.ToString())
                                            ? (double?)null : seriesJson["bayesian_rating"]?.Value<double?>();
            _seriesDetailsModel.ImageUrl = string.IsNullOrWhiteSpace(seriesJson["image"]?["url"]?["original"]?.ToString())
                                            ? (string?)null : seriesJson["image"]?["url"]?["original"]?.Value<string?>();
            _seriesDetailsModel.LatestChapter = string.IsNullOrWhiteSpace(seriesJson["latest_chapter"]?.ToString())
                                            ? (int?)null : seriesJson["latest_chapter"]?.Value<int?>();
            _seriesDetailsModel.OriginStatus = string.IsNullOrWhiteSpace(seriesJson["status"]?.ToString())
                                            ? (string?)null : seriesJson["status"]?.Value<string?>();
            _seriesDetailsModel.OriginCompleted = _seriesDetailsModel.OriginStatus == null ? (int?)null : _seriesDetailsModel.OriginStatus.Contains("Complete") ? 1 : 0;
            //_seriesDetailsModel.Licensed = seriesJson["licensed"]?.Value<string?>() == null ? (int?)null : seriesJson["licensed"]?.Value<string?>() == "true" ? 1 : 0;
            _seriesDetailsModel.Licensed = string.IsNullOrWhiteSpace(seriesJson["licensed"]?.ToString())
                                            ? (int?)null : seriesJson["licensed"]?.Value<string?>() == "true" ? 1 : 0;
            _seriesDetailsModel.ScanlationCompleted = string.IsNullOrWhiteSpace(seriesJson["completed"]?.ToString())
                                            ? (int?)null : seriesJson["completed"]?.Value<string>() == "true" ? 1 : 0;
            _seriesDetailsModel.AnimeStart = string.IsNullOrWhiteSpace(seriesJson["anime"]!["start"]?.ToString())
                                            ? (string?)null : seriesJson["anime"]!["start"]?.Value<string?>();
            _seriesDetailsModel.AnimeEnd = string.IsNullOrWhiteSpace(seriesJson["anime"]!["end"]?.ToString())
                                            ? (string?)null : seriesJson["anime"]!["end"]?.Value<string?>();
            _seriesDetailsModel.LastUpdated = string.IsNullOrWhiteSpace(seriesJson["last_updated"]?["as_rfc3339"]?.ToString())
                                            ? (string?)null : seriesJson["last_updated"]?["as_rfc3339"]?.Value<string?>();

            await _dataInsertHelper.InsertSeriesDetails(connection, _seriesDetailsModel);

            foreach (var genre in seriesJson["genres"]!)
            {
                string genreName = genre["genre"]!.Value<string>()!;
                await _dataInsertHelper.InsertGenres(connection, genreName);
                var genreId = await _dataRetrieveHelper.RetrieveGenreId(connection, genreName);
                await _dataInsertHelper.InsertSeriesGenres(connection, seriesId, genreId);
            }

            foreach (var category in seriesJson["categories"]!)
            {
                string categoryName = category["category"]!.Value<string>()!;
                await _dataInsertHelper.InsertCategories(connection, categoryName);
                var categoryId = await _dataRetrieveHelper.RetrieveCategoryId(connection, categoryName);
                await _dataInsertHelper.InsertSeriesCategories(connection, seriesId, categoryId);
            }

            foreach (var relation in seriesJson["related_series"]!)
            {
                _seriesRelationsModel.SeriesId = seriesId;
                _seriesRelationsModel.RelationType = relation["relation_type"]!.Value<string>();
                long relatedSeriesNewId = relation["related_series_id"]!.Value<long>();
                _seriesRelationsModel.RelatedSeriesId = await _dataRetrieveHelper.RetrieveSeriesIdFromNew(connection, relatedSeriesNewId);
                _seriesRelationsModel.RelatedSeriesName = relation["related_series_name"]!.Value<string>();
                await _dataInsertHelper.InsertSeriesRelations(connection, _seriesRelationsModel);
            }

            foreach (var synonym in seriesJson["associated"]!)
            {
                await _dataInsertHelper.InsertSeriesSynonyms(connection, seriesId, synonym["title"]!.Value<string>()!);
            }
        }
    }
}