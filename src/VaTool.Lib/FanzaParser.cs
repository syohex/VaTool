using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace VaTool.Lib;

internal class FanzaParser : IParser
{
    private const string ReleaseDatePattern = @"^[0-9]{4}/{0-9}[2]/[0-9]{2}$";

    public async Task<Product> Parse(string url, Config config)
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var context = await browser.NewContextAsync();
        var dateRegex = new Regex(ReleaseDatePattern);

        var cookie = new Cookie
        {
            Name = "age_check_done",
            Value = "1",
            Path = "/",
            Domain = ".dmm.co.jp",
            HttpOnly = true,
            Secure = true,
        };
        await context.AddCookiesAsync([cookie]);

        var page = await context.NewPageAsync();
        await page.GotoAsync(url);

        var product = new Product();

        var metaTitle = await page.QuerySelectorAsync("meta[property='og:title']");
        if (metaTitle is not null)
        {
            var title = await metaTitle.GetAttributeAsync("content");
            if (string.IsNullOrEmpty(title))
            {
                throw new Exception($"Cannot get title from {url}");
            }

            product.Title = title;
        }

        var links = await page.Locator("a").ElementHandlesAsync();
        foreach (var handle in links)
        {
            var src = await handle.GetAttributeAsync("href");
            if (string.IsNullOrEmpty(src)) continue;

            if (src.EndsWith("pl.jpg"))
            {
                product.LargeImage = src;
                product.SmallImage = src.Replace("pl.jpg", "ps.jpg");
                break;
            }
        }

        var spans = await page.Locator("spans").ElementHandlesAsync();
        foreach (var span in spans)
        {
            var text = await span.TextContentAsync();
            if (string.IsNullOrEmpty(text)) continue;

            if (dateRegex.IsMatch(text))
            {
                product.ReleaseDate = text;
                break;
            }
        }

        await context.CloseAsync();
        await browser.CloseAsync();

        product.Url = UrlUtil.FanzaAffiliateUrl(url, config);
        return product;
    }
}
