using CpmConverterCli.Utilities;

namespace CpmConverterCli.Tests.Unit;

using FluentAssertions;
using System.Collections.Generic;
using Xunit;

public class ProjectPackageReferenceUtilityTests
{
    // [Fact]
    // public void PopulateAllPackageReferences_ShouldPopulateAllPackageReferences()
    // {
    //     // Arrange
    //     var packageReferences = new List<string>
    //     {
    //         "<PackageReference Include=\"PackageA\" Version=\"1.0.0\"/>",
    //         "<PackageReference Include=\"PackageB\" Version=\"2.0.0\"/>",
    //     };
    //
    //     // Act
    //     ProjectPackageReferenceUtility utility = new (packageReferences);
    //
    //     // Assert
    //     utility.AllPackageReferences.Should().HaveCount(2);
    //     utility.AllPackageReferences.Should().Contain(pr => pr.Name == "PackageA" && pr.Version == "1.0.0");
    //     utility.AllPackageReferences.Should().Contain(pr => pr.Name == "PackageB" && pr.Version == "2.0.0");
    // }
    //
    // [Fact]
    // public void PopulatePackageReferencesUniqueVersions_ShouldPopulateUniqueVersions()
    // {
    //     // Arrange
    //     var packageReferences = new List<string>
    //     {
    //         "<PackageReference Include=\"PackageA\" Version=\"1.0.0\"/>",
    //         "<PackageReference Include=\"PackageA\" Version=\"1.0.0\"/>",
    //         "<PackageReference Include=\"PackageB\" Version=\"2.0.0\"/>",
    //     };
    //
    //     // Act
    //     ProjectPackageReferenceUtility utility = new (packageReferences);
    //
    //     // Assert
    //     utility.PackageReferencesUniqueVersions.Should().HaveCount(2);
    //     utility.PackageReferencesUniqueVersions.Should().Contain(pr => pr.Name == "PackageA" && pr.Version == "1.0.0");
    //     utility.PackageReferencesUniqueVersions.Should().Contain(pr => pr.Name == "PackageB" && pr.Version == "2.0.0");
    // }
    //
    // [Fact]
    // public void PopulatePackageReferencesDuplicateVersions_ShouldPopulateDuplicateVersions()
    // {
    //     // Arrange
    //     var packageReferences = new List<string>
    //     {
    //         "<PackageReference Include=\"PackageA\" Version=\"1.0.0\"/>",
    //         "<PackageReference Include=\"PackageA\" Version=\"1.0.0\"/>",
    //         "<PackageReference Include=\"PackageB\" Version=\"2.0.0\"/>",
    //     };
    //
    //     // Act
    //     ProjectPackageReferenceUtility utility = new (packageReferences);
    //
    //     // Assert
    //     utility.PackageReferencesDuplicateVersions.Should().HaveCount(2);
    //     utility.PackageReferencesDuplicateVersions.Should().Contain(pr => pr.Name == "PackageA" && pr.Version == "1.0.0");
    // }


    [Fact]
    public void MarkOverridesInPackageReferences_ShouldMarkOverridesCorrectly()
    {
        // Arrange
        var packageReferences = new List<string>
        {
            "<PackageReference Include=\"PackageA\" Version=\"1.0.0\"/>",
            "<PackageReference Include=\"PackageA\" Version=\"2.0.0\"/>",
            "<PackageReference Include=\"PackageB\" Version=\"1.0.0\"/>",
        };

        ProjectPackageReferenceUtility utility = new(packageReferences);

        // Act
        utility.MarkOverridesInPackageReferences(utility.AllPackageReferences);

        // Assert
        utility.AllPackageReferences.Should().Contain(pr => pr.Name == "PackageA" && pr.Version == "1.0.0" && pr.IsOverride);
        utility.AllPackageReferences.Should().Contain(pr => pr.Name == "PackageA" && pr.Version == "2.0.0" && pr.IsOverride);
        utility.AllPackageReferences.Should().Contain(pr => pr.Name == "PackageB" && pr.Version == "1.0.0" && !pr.IsOverride);
    }
}