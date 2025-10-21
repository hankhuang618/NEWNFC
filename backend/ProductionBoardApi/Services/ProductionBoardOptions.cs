namespace ProductionBoardApi.Services;

public class ProductionBoardOptions
{
    public const string ConfigurationSectionName = "ProductionBoard";

    public string? ConnectionString { get; set; }
}
