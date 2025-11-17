namespace VaTool.Lib;

public interface IParser
{
    Task<Product> Parse(string url, Config config);
}
