using System.Diagnostics;
using System.Net.Http;
using HtmlAgilityPack;

namespace SpixManga.Misc;

public class ScrapeManga
{
    public async Task<Dictionary<string, int>> GetGenres()
    {
        string url = "https://www.mangaupdates.com/meta/genres";
        HttpClient client = new HttpClient();
        Dictionary<string, int> genreDict = [];

        var response = await client.GetStringAsync(url);

        HtmlDocument document = new HtmlDocument();
        document.LoadHtml(response);

        var links = document.DocumentNode.SelectNodes("//a[contains(text(), 'Search Series for')]");

        if (links != null)
        {
            foreach (var link in links)
            {
                string genre = link.InnerText.Split(new string[] { "Search Series for ", " (" },
                    StringSplitOptions.RemoveEmptyEntries)[0];
                var numberNode = link.SelectSingleNode(".//b");
                int totalCount = int.Parse(numberNode.InnerText);
                genreDict.Add(genre, totalCount);
            }
        }

        return genreDict;
    }

    public async Task<List<string>> GetLinks(Dictionary<string, int> genresDict)
    {
        HttpClient client = new HttpClient();

        var sortedAsc = genresDict.OrderBy(pair => pair.Value).ToList();
        var sortedDict = sortedAsc.ToDictionary(pair => pair.Key, pair => pair.Value);

        List<string> genres = new List<string>(sortedDict.Keys);
        List<int> totalCounts = new List<int>(sortedDict.Values);
        List<string> excludeGenres = [];

        List<string> mangaSet = new List<string>();
        int i = 0;
        string genreStr = "";
        bool z = false;

        List<string> tempGenres = new List<string>(genres);
        while (genres.Count > 0)
        {
            if (i == genres.Count)
            {
                i = 0;
                for (int j = 1; j < genres.Count; j++)
                {
                    genreStr = string.Join("+", genres[i].Split(' '));
                    excludeGenres.Add(genres[i + j]);
                    int totalPages1 = await checkPage(genreStr, excludeGenres, client, mangaSet);

                    genreStr = string.Join("+", genres[i].Split(' ')) + "_" +
                               string.Join("+", genres[i + j].Split(' '));
                    excludeGenres.Remove(excludeGenres.Last());
                    int totalPages2 = await checkPage(genreStr, excludeGenres, client, mangaSet);

                    if (totalPages1 < 100 && totalPages2 < 100)
                    {
                        excludeGenres.Add(genres[i]);
                        mangaSet.Add(genres[i] + "-" + genres[i + j] + " " + totalPages1.ToString());
                        mangaSet.Add(genres[i] + "-" + genres[i + j] + " " + totalPages2.ToString());
                        genres.RemoveAt(i);
                        i = 0;
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }

                if (tempGenres.SequenceEqual(genres))
                {
                    z = true;
                }
            }

            genreStr = string.Join("+", genres[i].Split(' '));
            int totalPages = await checkPage(genreStr, excludeGenres, client, mangaSet);
            if (totalPages < 100)
            {
                excludeGenres.Add(genres[i]);
                mangaSet.Add(genres[i] + " " + totalPages.ToString());
                genres.RemoveAt(i);
                i = 0;
            }
            else
            {
                i++;
            }

            await Task.Delay(1000);
            Debug.WriteLine(string.Join(",", genres));
            Debug.WriteLine(string.Join(",", excludeGenres));

            if (z == true)
            {
                break;
            }
            else
            {
                tempGenres = new List<string>(genres);
            }
        }

        for (int j = 0; j < genres.Count; j++)
        {
            mangaSet.Add(genres[j]);
        }

        Debug.WriteLine(string.Join(",", genres));
        Debug.WriteLine(string.Join(",", excludeGenres));
        return mangaSet;
    }

    public async Task<int> checkPage(string genreStr, List<string> excludeGenres, HttpClient client,
        List<string> mangaSet)
    {
        string url = $"https://www.mangaupdates.com/series?genre={genreStr}&perpage=100";
        string excludeUrl = string.Join("_", excludeGenres.Select(s => string.Join("+", s.Split(' '))));

        if (excludeGenres.Count() > 0)
        {
            url += $"&exclude_genre={excludeUrl}";
        }

        int totalPages = 0;

        Debug.WriteLine(url);
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

    public async Task<HashSet<string>> visitPage(HttpResponseMessage response)
    {
        HashSet<string> pageMangaSet = [];
        HtmlDocument document = new HtmlDocument();
        document.LoadHtml(await response.Content.ReadAsStringAsync());
        var links = document.DocumentNode.SelectNodes("//a[@href]");

        if (links != null)
        {
            foreach (var link in links)
            {
                string href = link.GetAttributeValue("href", string.Empty);
                string baseString = "https://www.mangaupdates.com/series/";

                if (!string.IsNullOrEmpty(href) && href.StartsWith(baseString))
                {
                    string relativePath = href.Substring(baseString.Length);
                    pageMangaSet.Add(href);
                }
            }
        }

        return pageMangaSet;
    }
}