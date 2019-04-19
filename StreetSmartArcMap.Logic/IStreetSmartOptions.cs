namespace StreetSmartArcMap.Logic
{
    public interface IStreetSmartOptions
    {
        string EpsgCode { get; set; }
        string Locale { get; set; }
        string Database { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string ApiKey { get; set; }
    }
}
