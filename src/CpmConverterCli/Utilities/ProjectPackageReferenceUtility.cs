using System.Text;
using System.Text.RegularExpressions;

namespace CpmConverterCli.Utilities;

public partial class ProjectPackageReferenceUtility
{
    [GeneratedRegex("<PackageReference Include=\"(?<name>[^\"]+)\" Version=\"(?<version>[^\"]+)\"\\s*/?>")]
    private static partial Regex PackageReferenceRegex();

    public ProjectPackageReferenceUtility(List<string> packageReferences)
    {
        PopulateAllPackageReferences(packageReferences);
    }

    public HashSet<PackageReference> AllPackageReferences { get; } = new();

    public string GetPropsFileVersionSection()
    {
        string spaces = "    ";
        StringBuilder stringBuilder = new();
        for (int i = 0; i < AllPackageReferences.Count; i++)
        {
            if (i > 0) spaces = "      ";

            PackageReference packageReference = AllPackageReferences.ElementAt(i);
            stringBuilder.Append($"{spaces}<PackageVersion Include=\"{packageReference.Name}\" Version=\"{packageReference.Version}\"/>\n");
        }

        return stringBuilder.ToString();
    }

    #region Init Code


    private void PopulateAllPackageReferences(List<string> packageReferences)
    {
        foreach (string packageReferenceCsprojLine in packageReferences)
        {
            PackageReference? packageReference = GetPackageReferenceFromCsprojLine(packageReferenceCsprojLine);
            if (packageReference is null) continue;

            AllPackageReferences.Add(packageReference);
        }

        MarkOverridesInPackageReferences(AllPackageReferences);
    }

    public PackageReference? GetPackageReferenceFromCsprojLine(string packageReference)
    {
        packageReference = packageReference.Trim();

        Regex regex = PackageReferenceRegex();
        Match match = regex.Match(packageReference);
        if (!match.Success) return null;

        string packageName = match.Groups["name"].Value;
        string packageVersion = match.Groups["version"].Value;

        return new PackageReference { Name = packageName, Version = packageVersion };
    }

    internal void MarkOverridesInPackageReferences(HashSet<PackageReference> allPackageReferences)
    {
        List<string> duplicatePackageNames = allPackageReferences
            .GroupBy(pr => pr.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        // Are the names packages different versions?
        foreach (string duplicatePackageName in duplicatePackageNames)
        {
            List<string> duplicatePackageWithDifferentVersions = allPackageReferences
                .Where(pr => pr.Name == duplicatePackageName)
                .Select(pr => pr.Version)
                .Distinct()
                .ToList();

            if (duplicatePackageWithDifferentVersions.Count == 1)
            {
                continue;
            }


            //query AllPackageReferences for the PackageReference with the same name
            List<PackageReference> allPackageReferencesWithTheSameName = allPackageReferences
                .Where(pr => pr.Name == duplicatePackageName)
                .ToList();

            // Order by version and get the highest version
            PackageReference packageReferenceWithHighestVersion = allPackageReferencesWithTheSameName
                .OrderByDescending(pr => pr.Version)
                .First();

            // if it is not the highest version, mark it as an override
            foreach (PackageReference packageReference in allPackageReferencesWithTheSameName)
            {
                if (packageReference.Version != packageReferenceWithHighestVersion.Version)
                {
                    packageReference.IsOverride = true;
                }
            }
        }

    }

    #endregion Init Code
}