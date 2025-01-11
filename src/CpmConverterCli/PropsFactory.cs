using CpmConverterCli.Exceptions;
using CpmConverterCli.Utilities;

namespace CpmConverterCli;

public class PropsFactory
{
    private readonly string _solutionDirectory;
    private readonly ProjectPackageReferenceUtility _projectPackageReferenceUtility;

    public PropsFactory(string solutionFilePath)
    {
        _solutionDirectory = Path.GetDirectoryName(solutionFilePath) ?? throw new FriendlyException("Invalid solution file path");
        if (SolutionPathIsValid(solutionFilePath))
        {
            List<string> projectPackageReferences = GetPackageReferenceStringsFromAllProjectFiles();

            _projectPackageReferenceUtility = new(projectPackageReferences);
            string propsFileVersionSection = _projectPackageReferenceUtility.GetPropsFileVersionSection();

            CreatePropsFile(propsFileVersionSection);

            UpdateCsprojFilesToCpm();
        }
        else
        {
            throw new FriendlyException($"Invalid solution file path: {nameof(solutionFilePath)}");
        }
    }

    private void UpdateCsprojFilesToCpm()
    {
        try
        {
            string[] csprojFiles = Directory.GetFiles(_solutionDirectory, "*.csproj", SearchOption.AllDirectories);
            foreach (string csprojFile in csprojFiles)
            {
                string[] lines = File.ReadAllLines(csprojFile);
                bool fileUpdated = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    PackageReference? packageRef = _projectPackageReferenceUtility.GetPackageReferenceFromCsprojLine(lines[i]);
                    if (packageRef is null) continue;

                    var matchingReferencesWithTheSameName = _projectPackageReferenceUtility.AllPackageReferences
                        .Where(x => x.Name == packageRef.Name)
                        .ToList();

                    if (matchingReferencesWithTheSameName.Count > 1)
                    {
                        PackageReference? currentPackageReference = matchingReferencesWithTheSameName
                            .First(x => x.Version == packageRef.Version);

                        if (currentPackageReference.IsOverride)
                        {
                            lines[i] = $"<PackageReference Include=\"{packageRef.Name}\" VersionOverride=\"{packageRef.Version}\" />";
                        }
                        else
                        {
                            lines[i] = $"<PackageReference Include=\"{packageRef.Name}\" />";
                        }
                    }
                    else
                    {
                        lines[i] = $"<PackageReference Include=\"{packageRef.Name}\" />";
                    }

                    fileUpdated = true;
                }

                if (fileUpdated)
                {
                    File.WriteAllLines(csprojFile, lines);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private List<string> GetPackageReferenceStringsFromAllProjectFiles()
    {
        List<string> packageReferences = new();
        string[] csprojFiles = Directory.GetFiles(_solutionDirectory, "*.csproj", SearchOption.AllDirectories);

        foreach (string csprojFile in csprojFiles)
        {
            string csprojContent = File.ReadAllText(csprojFile);
            string[] lines = csprojContent.Split(Environment.NewLine);
            foreach (string line in lines)
            {
                if (line.Contains("PackageReference"))
                {
                    // string packageVersion = line.Split("Version=\"")[1].Split("\"")[0];
                    packageReferences.Add(line);
                }
            }
        }

        return packageReferences;
    }

    private void CreatePropsFile(string versionSection)
    {
        string propsFilePath = Path.Combine(_solutionDirectory, "Directory.Build.props");
        if (File.Exists(propsFilePath))
        {
            throw new FriendlyException("Props file already exists");
        }

        string propsFileContent = GetBasePropsFileContent(versionSection);

        File.WriteAllText(propsFilePath, propsFileContent);
    }

    private string GetBasePropsFileContent(string packageVersionSection)
    {
        string propsFileBaseContent = $"""
                                       <Project>
                                         <PropertyGroup>
                                           <TargetFramework>net8.0</TargetFramework>
                                           <ImplicitUsings>enable</ImplicitUsings>
                                           <Nullable>enable</Nullable>
                                           <WarningsAsErrors>nullable</WarningsAsErrors>
                                           <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
                                           <CentralPackageTransitivePinningEnable>true</CentralPackageTransitivePinningEnable>
                                         </PropertyGroup>
                                         <ItemGroup>
                                         {packageVersionSection?.TrimEnd()}
                                         </ItemGroup>
                                       </Project>
                                       """;


        return propsFileBaseContent;
    }

    private bool SolutionPathIsValid(string solutionFilePath)
    {
        return solutionFilePath.EndsWith(".sln") && File.Exists(solutionFilePath);
    }
}