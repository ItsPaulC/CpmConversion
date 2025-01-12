using CpmConverterCli.Utilities;

namespace CpmConverterCli.Tests.Unit;

using FluentAssertions;
using System.Collections.Generic;
using VerifyXunit;
using Xunit;

public class ProjectPackageReferenceUtilityTests
{
    [Fact]
    public async Task MarkOverridesInPackageReferences_ShouldMarkLowerVersionsAsOverrides()
    {
        // Arrange
        var packageReferences = new List<string>
        {
            "<PackageReference Include=\"PackageA\" Version=\"1.0.0\" />",
            "<PackageReference Include=\"PackageA\" Version=\"2.0.0\" />",
            "<PackageReference Include=\"PackageB\" Version=\"1.0.0\" />"
        };
        var utility = new ProjectPackageReferenceUtility(packageReferences);

        // Act
        utility.MarkOverridesInPackageReferences(utility.AllPackageReferences);

        // Assert
        await Verify(utility.AllPackageReferences);
        // var packageA1 = utility.AllPackageReferences.First(pr => pr is { Name: "PackageA", Version: "1.0.0" });
        // var packageA2 = utility.AllPackageReferences.First(pr => pr is { Name: "PackageA", Version: "2.0.0" });
        // var packageB = utility.AllPackageReferences.First(pr => pr is { Name: "PackageB", Version: "1.0.0" });
        //
        // packageA1.IsOverride.Should().BeTrue();
        // packageA2.IsOverride.Should().BeFalse();
        // packageB.IsOverride.Should().BeFalse();
    }
}