using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace VaTool.Lib;

internal class FanzaParser : IParser
{
    private const string ReleaseDatePattern = @"^[0-9]{4}/[0-9]{2}/[0-9]{2}$";

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

        // 'load' event never happens with PlayWright, wait for 'DOMContentLoaded' and title being available instead
        var gotoOptions = new PageGotoOptions
        {
            Timeout = 10.0f * 1000.0f,
            WaitUntil = WaitUntilState.DOMContentLoaded
        };

        await page.GotoAsync(url, gotoOptions);

        var selector = "meta[property='og:title']";
        await page.WaitForFunctionAsync("selector => !!document.querySelector(selector)", selector);

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

        var links = await page.Locator("img").ElementHandlesAsync();
        foreach (var link in links)
        {
            var src = await link.GetAttributeAsync("src");
            if (string.IsNullOrEmpty(src)) continue;

            if (src.Contains("pl.jpg"))
            {
                var pos = src.IndexOf('?');
                if (pos >= 0)
                {
                    src = src[..pos];
                }

                product.LargeImage = src;
                product.SmallImage = product.LargeImage.Replace("pl.jpg", "ps.jpg");
                break;
            }
        }

        var spans = await page.Locator("span").ElementHandlesAsync();
        foreach (var span in spans)
        {
            var text = await span.InnerTextAsync();
            if (string.IsNullOrEmpty(text)) continue;

            if (dateRegex.IsMatch(text))
            {
                product.ReleaseDate = DateUtil.FormatDate(text);
                break;
            }
        }

        await context.CloseAsync();
        await browser.CloseAsync();

        product.Url = UrlUtil.FanzaAffiliateUrl(url, config);
        return product;
    }
}
