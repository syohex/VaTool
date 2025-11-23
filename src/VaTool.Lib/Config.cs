namespace VaTool.Lib;

public struct Config
{
    public AffiliateInfo Dmm { get; set; }
    public AffiliateInfo Sokmil { get; set; }

    public struct AffiliateInfo
    {
        public string Id { get; set; }
    }

    public static string DefaultPath
    {
        get
        {
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (string.IsNullOrEmpty(homeDir))
            {
                throw new Exception("Cannot find home directory");
            }

            return Path.Join(homeDir, ".config", "blog", "config.yaml");
        }
    }
}
