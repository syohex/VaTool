using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace VaTool.Lib;

public class PageLoader
{
    public static async Task<Page> Load(string pageFile)
    {
        var yaml = await File.ReadAllTextAsync(pageFile);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        return deserializer.Deserialize<Page>(yaml);
    }
}
