using System.Diagnostics;
using System.Net.Http;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using SpixManga.Misc;

namespace SpixManga.GetData;

internal class ScrapeSeriesId
{
    private async Task<Dictionary<string, int>> GetGenres(HttpClient httpClient)
    {
        Dictionary<string, int> genresDict = new Dictionary<string, int>();
        string genreUrl = "https://api.mangaupdates.com/v1/genres";
        await InternetHelper.WaitForInternetAsync();
        HttpResponseMessage genreResponse = await httpClient.GetAsync(genreUrl);
        ;

        var genreContent = await genreResponse.Content.ReadAsStringAsync();
        var genreJson = JArray.Parse(genreContent);

        foreach (var genre in genreJson)
        {
            var genreName = (string)genre["genre"]!;
            int seriesCount = (int)genre["stats"]!["series"]!;
            genresDict[genreName] = seriesCount;
        }

        return genresDict;
    }

    internal async Task<HashSet<string>> GetSeriesId(HttpClient httpClient)
    {
        Dictionary<string, int> genresDict = await GetGenres(httpClient);
        var sortedAsc = genresDict.OrderBy(pair => pair.Value).ToList();
        var sortedDict = sortedAsc.ToDictionary(pair => pair.Key, pair => pair.Value);

        List<string> genres = new List<string>(sortedDict.Keys);
        List<string> excludeGenres = [];

        HashSet<string> mangaSet = new HashSet<string>();
        var resultSet = new HashSet<string>();

        while (genres.Count > 0)
        {
            string genreStr = "&genre=" + string.Join("+", genres[0].Split(' '));
            string url = urlBuilder(genreStr, excludeGenres);
            int totalPages = await checkTotalPages(url, httpClient);

            if (totalPages < 100)
            {
                await simpleExtract(httpClient, url, resultSet, excludeGenres, genres);
            }
            else
            {
                bool successFlag = await complexExtract(httpClient, resultSet, excludeGenres, genres);
                if (successFlag)
                {
                    await letterExtract(httpClient, resultSet, excludeGenres, genres);
                }
            }
        }

        await exceptGenreExtract(httpClient, resultSet, excludeGenres);

        return resultSet;
    }

    internal async Task<HashSet<string>> UpdateSeriesId(HttpClient httpClient, HashSet<string> oldSeriesSet)
    {
        HashSet<string> resultSet = new();
        bool breakFlag = false;
        int page = 1;
        while (true)
        {
            string url = $"https://www.mangaupdates.com/stats/new?orderby=date&page={page}&perpage=100";
            await InternetHelper.WaitForInternetAsync();
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url);
            var pageSeriesSet = await visitPage(httpResponseMessage);
            foreach (var item in pageSeriesSet)
            {
                if (oldSeriesSet.Contains(item))
                {
                    breakFlag = true;
                    Debug.WriteLine(item);
                    break;
                }
                else
                {
                    resultSet.Add(item);
                }
            }

            if (breakFlag == true)
            {
                break;
            }

            page++;
            await Task.Delay(1000);
        }

        return resultSet;
    }

    private async Task<int> checkTotalPages(string url, HttpClient client)
    {
        int totalPages = 1;
        await InternetHelper.WaitForInternetAsync();
        var response = await client.GetStringAsync(url);
        HtmlDocument document = new HtmlDocument();
        document.LoadHtml(response);

        var totalPageNode = document.DocumentNode.SelectSingleNode("//span[contains(text(), 'Pages')]");
        if (totalPageNode != null)
        {
            string text = totalPageNode.InnerText.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                string numberStr = text.Split('(')[1].Split(')')[0];
                totalPages = int.Parse(numberStr);
            }
        }

        return totalPages;
    }

    private async Task<HashSet<string>> visitPage(HttpResponseMessage response)
    {
        HashSet<string> pageMangaSet = [];

        HtmlDocument document = new HtmlDocument();
        document.LoadHtml(await response.Content.ReadAsStringAsync());
        var links = document.DocumentNode
            .SelectNodes(
                "//a[starts-with(@href, 'https://www.mangaupdates.com/series/') and @title='Click for Series Info']")
            ?.Select(node => node.GetAttributeValue("href", ""))
            .ToList();

        if (links != null && links.Count > 0)
        {
            foreach (var link in links)
            {
                string[] parts = link.Split('/');
                pageMangaSet.Add(parts[4]);
            }
        }

        return pageMangaSet;
    }

    private string urlBuilder(string genreStr, List<string> excludeGenres)
    {
        string url = $"https://www.mangaupdates.com/series?perpage=100{genreStr}";
        string excludeGenreUrl = string.Join("_", excludeGenres.Select(s => string.Join("+", s.Split(' '))));

        if (excludeGenres.Count() > 0)
        {
            url += $"&exclude_genre={excludeGenreUrl}";
        }

        return url;
    }

    private async Task simpleExtract(HttpClient httpClient, string url, HashSet<string> resultSet,
        List<string> excludeGenres, List<string> genres)
    {
        for (int page = 1; page < 101; page++)
        {
            await InternetHelper.WaitForInternetAsync();
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url + $"&page={page}");
            var pageMangaSet = await visitPage(httpResponseMessage);
            if (pageMangaSet.Count() == 0)
            {
                break;
            }

            foreach (var item in pageMangaSet)
            {
                resultSet.Add(item);
            }

            await Task.Delay(1000);
        }

        excludeGenres.Add(genres[0]);
        genres.RemoveAt(0);
        await Task.Delay(1000);
    }

    private async Task<bool> complexExtract(HttpClient httpClient, HashSet<string> resultSet,
        List<string> excludeGenres, List<string> genres)
    {
        string genreStr = "";
        for (int i = 0; i < genres.Count; i++)
        {
            for (int j = i + 1; j < genres.Count; j++)
            {
                genreStr = "&genre=" + string.Join("+", genres[i].Split(' '));
                excludeGenres.Add(genres[j]);
                string url1 = urlBuilder(genreStr, excludeGenres);
                int totalPages1 = await checkTotalPages(url1, httpClient);

                genreStr = "&genre=" + string.Join("+", genres[i].Split(' ')) + "_" +
                           string.Join("+", genres[j].Split(' '));
                excludeGenres.Remove(excludeGenres.Last());
                string url2 = urlBuilder(genreStr, excludeGenres);
                int totalPages2 = await checkTotalPages(url2, httpClient);

                if (totalPages1 < 100 && totalPages2 < 100)
                {
                    for (int page = 1; page < 101; page++)
                    {
                        await InternetHelper.WaitForInternetAsync();
                        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url1 + $"&page={page}");
                        var pageMangaSet = await visitPage(httpResponseMessage);
                        if (pageMangaSet.Count() == 0)
                        {
                            break;
                        }

                        foreach (var item in pageMangaSet)
                        {
                            resultSet.Add(item);
                        }

                        await Task.Delay(1000);
                    }

                    await Task.Delay(1000);


                    for (int page = 1; page < 101; page++)
                    {
                        await InternetHelper.WaitForInternetAsync();
                        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url2 + $"&page={page}");
                        var pageMangaSet = await visitPage(httpResponseMessage);
                        if (pageMangaSet.Count() == 0)
                        {
                            break;
                        }

                        foreach (var item in pageMangaSet)
                        {
                            resultSet.Add(item);
                        }

                        await Task.Delay(1000);
                    }

                    excludeGenres.Add(genres[i]);
                    genres.RemoveAt(i);
                    await Task.Delay(1000);
                    return true;
                }

                await Task.Delay(1000);
            }
        }

        return false;
    }

    private async Task letterExtract(HttpClient httpClient, HashSet<string> resultSet,
        List<string> excludeGenres, List<string> genres)
    {
        string genreStr = "&genre=" + string.Join("+", genres[0].Split(' '));
        string url = urlBuilder(genreStr, excludeGenres);
        for (char c = 'A'; c <= 'Z'; c++)
        {
            for (int page = 1; page < 101; page++)
            {
                await InternetHelper.WaitForInternetAsync();
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url + $"&letter={c}&page={page}");
                var pageMangaSet = await visitPage(httpResponseMessage);
                if (pageMangaSet.Count() == 0)
                {
                    break;
                }

                foreach (var item in pageMangaSet)
                {
                    resultSet.Add(item);
                }

                await Task.Delay(1000);
            }

            await Task.Delay(1000);
        }

        await Task.Delay(1000);
        
        for (int page = 1; page < 101; page++)
        {
            url = urlBuilder(genreStr, excludeGenres);
            await InternetHelper.WaitForInternetAsync();
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url + $"&page={page}");
            var pageMangaSet = await visitPage(httpResponseMessage);
            if (pageMangaSet.Count() == 0)
            {
                break;
            }

            foreach (var item in pageMangaSet)
            {
                resultSet.Add(item);
            }

            await Task.Delay(1000);
        }

        await Task.Delay(1000);
        
        for (int year = 1900; year < 2026; year++)
        {
            for (int page = 1; page < 101; page++)
            {
                await InternetHelper.WaitForInternetAsync();
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url + $"&year={year}&page={page}");
                var pageMangaSet = await visitPage(httpResponseMessage);
                if (pageMangaSet.Count() == 0)
                {
                    break;
                }

                foreach (var item in pageMangaSet)
                {
                    resultSet.Add(item);
                }

                await Task.Delay(1000);
            }

            await Task.Delay(1000);
        }

        excludeGenres.Add(genres[0]);
        genres.Remove(genres[0]);
        await Task.Delay(1000);
    }

    private async Task exceptGenreExtract(HttpClient httpClient, HashSet<string> resultSet,
        List<string> excludeGenres)
    {
        string url = urlBuilder("", excludeGenres);
        for (int page = 1; page < 101; page++)
        {
            await InternetHelper.WaitForInternetAsync();
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url + $"&page={page}");
            var pageSeriesSet = await visitPage(httpResponseMessage);
            if (pageSeriesSet.Count() == 0)
            {
                break;
            }

            foreach (var item in pageSeriesSet)
            {
                resultSet.Add(item);
            }

            await Task.Delay(1000);
        }

        await Task.Delay(1000);
    }
}
