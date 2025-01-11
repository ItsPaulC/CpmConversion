namespace CpmConverterCli.Utilities;

public class PackageReference
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;

    public bool IsOverride { get; set; }
}