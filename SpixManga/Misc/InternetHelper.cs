using System.Net.Http;
using System.Net.NetworkInformation;

namespace SpixManga.Misc;

public static class InternetHelper
{
    private static readonly string[] TestUrls =
    {
        "https://www.google.com/generate_204",
        "https://www.cloudflare.com",
        "https://clients3.google.com/generate_204",
        "https://www.bing.com"
    };

    // Checks for both network and actual internet availability with fallback URLs
    private static async Task<bool> IsInternetAvailableAsync()
    {
        if (!NetworkInterface.GetIsNetworkAvailable())
        { 
            return false; 
        }

        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

        foreach (var url in TestUrls)
        {
            try
            {
                using var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch
            {
                // Ignore and try next URL
            }
        }

        return false;
    }

    // Waits and retries until internet becomes available
    public static async Task WaitForInternetAsync(int retryDelayMs = 5000)
    {
        while (!await IsInternetAvailableAsync())
        {
            await Task.Delay(retryDelayMs);
        }
    }
}
