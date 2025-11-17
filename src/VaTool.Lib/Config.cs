namespace VaTool.Lib;

public struct Config
{
    public AffiliateInfo Dmm { get; set; }
    public AffiliateInfo Sokmil { get; set; }

    public struct AffiliateInfo
    {
        public string Id { get; set; }
    }
}
