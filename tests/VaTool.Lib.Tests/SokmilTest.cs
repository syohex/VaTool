using VaTool.Lib;

namespace VaTool.Tests;

public class SokmilTest
{
    [Fact]
    public async Task FetchAndParseTest()
    {
        var dummyConfig = new Config
        {
            Sokmil = new Config.AffiliateInfo
            {
                Id = "test001"
            },
            Dmm = new Config.AffiliateInfo
            {
                Id = "test002"
            }
        };

        var testUrl = "https://www.sokmil.com/av/_item/item368235.htm";
        var parser = ParserFactory.Create(testUrl);
        var res = await parser.Parse(testUrl, dummyConfig);
        Assert.NotEmpty(res.Title);
        Assert.NotEmpty(res.Image);
    }
}
