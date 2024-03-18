using System.Reflection;
using DecommTransformations.Versioning;

namespace DecommTransformations.Tests;

public class TransformerTests
{
    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
    
    private const string Version2310 = "2023.10";
    private static VersionRecords _versionRecord;
    
    private const string OriginalRepositoriesResource = "DecommTransformations.Tests.originaltestfiles.OriginalRepositories.config.test";
    private const string TransformedRepositoriesResource = "DecommTransformations.Tests.transformedtestfiles.TransformedRepositories.config.test";
    
    private const string OriginalContentSlnResource = "DecommTransformations.Tests.originaltestfiles.OriginalContent.sln.test";
    private const string TransformedContentSlnResource = "DecommTransformations.Tests.transformedtestfiles.TransformedContent.sln.test";

    private const string OriginalNugetResource = "DecommTransformations.Tests.originaltestfiles.OriginalNuget.ps1.test";
    private const string TransformedNugetResource = "DecommTransformations.Tests.transformedtestfiles.TransformedNuget.ps1.test";
    
    private const string OriginalCsprojResource = "DecommTransformations.Tests.originaltestfiles.OriginalHuxley.API.Version1.WebHost.csproj.test";
    private const string TransformedCsprojResource = "DecommTransformations.Tests.transformedtestfiles.TransformedHuxley.API.Version1.WebHost.csproj.test";
    
    
    [Theory]
    [InlineData(OriginalContentSlnResource)]
    [InlineData(OriginalRepositoriesResource)]
    [InlineData(OriginalNugetResource)]
    [InlineData(OriginalCsprojResource)]
    public void CheckFileForVersion_VersionExists_ReturnTrue(string file)
    {
    // arrange
    var originalContent = ReadEmbeddedResource(file).Split("\n");
    Transformer.IsValidVersion(Version2310, out _versionRecord);

    // act
    var result = Transformer.CheckFileForVersion(originalContent);
    // assert
    Assert.True(result);
    }

    [Theory]
    [InlineData(OriginalContentSlnResource, "2020.10")]
    [InlineData(OriginalRepositoriesResource, "2020.10")]
    [InlineData(OriginalNugetResource, "2020.10")]
    [InlineData(OriginalCsprojResource, "2020.10")]
    public void CheckFileForVersion_VersionDoesntExist_ReturnFalse(string file, string version)
    {
    // arrange
    var originalContent = ReadEmbeddedResource(file).Split("\n");
    Transformer.IsValidVersion(version, out _versionRecord);

    // act
    var result = Transformer.CheckFileForVersion(originalContent);
    // assert
    Assert.False(result);
    }
    
     [Fact]
    public void IfSolutionFileContainsVersion_AfterTransformation_ItWillNotContainVersionAnymore()
    {
        // arrange 
        var originalContent = ReadEmbeddedResource(OriginalContentSlnResource).Split("\n");
        var expectedTransformedContent = ReadEmbeddedResource(TransformedContentSlnResource);
        Transformer.IsValidVersion(Version2310, out _versionRecord);
        var transformer = new Transformer(_versionRecord);
        
        // act
        var actualTransformedSolutionFile = transformer.TransformSolutionFile(originalContent);
        
        // assert
        Assert.Equal(expectedTransformedContent, string.Join("\n", actualTransformedSolutionFile));
        Assert.DoesNotContain("2023X10", actualTransformedSolutionFile);
    }

    [Fact]
    public void TransformSolutionFile_IfInvalidFileContent_ReturnsEmptyStringArray()
    {
        // arrange
        var originalContent = new[] { "Invalid file content" };
        Transformer.IsValidVersion(Version2310, out _versionRecord);
        var transformer = new Transformer(_versionRecord);
    
        // act
        var result = transformer.TransformSolutionFile(originalContent);

        // assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void TransformSolutionFile_IfInvalidVersion_ReturnsEmptyStringArray()
    {
        // arrange
        var originalContent = ReadEmbeddedResource(OriginalContentSlnResource).Split("\n");
        Transformer.IsValidVersion("2000.02", out _versionRecord);
        var transformer = new Transformer(_versionRecord);
    
        // act
        var result = transformer.TransformSolutionFile(originalContent);

        // assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void IfConfigFileContainsVersion_AfterTransformation_ItWillNotContainVersionAnymore()
    {
        // arrange
        var originalContent = ReadEmbeddedResource(OriginalRepositoriesResource).Split("\n");
        var expectedTransformedContent = ReadEmbeddedResource(TransformedRepositoriesResource);
        
        Transformer.IsValidVersion(Version2310, out _versionRecord);
        var transformer = new Transformer(_versionRecord);
        
        // act
        var actualTransformedConfigFile = transformer.TransformConfigFile(originalContent);
    
        // assert
        Assert.Equal(expectedTransformedContent, string.Join("\n", actualTransformedConfigFile));
    }

    [Fact]
    public void TransformConfigFile_IfInvalidFileContent_ReturnsEmptyStringArray()
    {
        // arrange
        var originalContent = new[] { "Invalid file content" };
        Transformer.IsValidVersion(Version2310, out _versionRecord);
        var transformer = new Transformer(_versionRecord);
    
        // act
        var result = transformer.TransformSolutionFile(originalContent);

        // assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void TransformConfigFile_IfInvalidVersion_ReturnsEmptyStringArray()
    {
        // arrange
        var originalContent = ReadEmbeddedResource(OriginalContentSlnResource).Split("\n");
        Transformer.IsValidVersion("2000.02", out _versionRecord);
        var transformer = new Transformer(_versionRecord);
    
        // act
        var result = transformer.TransformSolutionFile(originalContent);

        // assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void IfNugetFileContainsVersion_AfterTransformation_ItWillNotContainVersionAnymore()
    {
        // arrange
        var originalContent = ReadEmbeddedResource(OriginalNugetResource).Split("\n");
        var expectedTransformedNugetContent = ReadEmbeddedResource(TransformedNugetResource);
                
        Transformer.IsValidVersion(Version2310, out _versionRecord);
        var transformer = new Transformer(_versionRecord);
        
        // act
        var actualTransformedNugetFile = transformer.TransformNugetFile(originalContent);
        
        // asserts
        Assert.Equal(expectedTransformedNugetContent, string.Join("\n", actualTransformedNugetFile));
        Assert.DoesNotContain("2023X10", actualTransformedNugetFile);
    }

    [Fact]
    public void IfNugetFileHasNoMatchingElements_ReturnOriginalContent()
    {
        // arrange
        var originalContent = ReadEmbeddedResource(OriginalNugetResource).Split("\n");
        Transformer.IsValidVersion("2023.09", out _versionRecord);
        var transformer = new Transformer(_versionRecord); 

        // act
        var actualResult = transformer.TransformNugetFile(originalContent);

        // assert
        Assert.Equal(originalContent, actualResult);
    }
    
    [Fact]
    public void IfCsProjFileContainsVersion_AfterTransformation_ItWillNotContainVersionAnymore()
    {
        // arrange
        var originalContent = ReadEmbeddedResource(OriginalCsprojResource).Split("\n");
        var expectedTransformedCsprojContent = ReadEmbeddedResource(TransformedCsprojResource);
                
        Transformer.IsValidVersion(Version2310, out _versionRecord);
        var transformer = new Transformer(_versionRecord);
        
        // act
        var actualTransformedCsprojFile = transformer.TransformCsprojFile(originalContent);
        
        // asserts
        Assert.Equal(expectedTransformedCsprojContent, string.Join("\n", actualTransformedCsprojFile));
        Assert.DoesNotContain("2023X10", actualTransformedCsprojFile);
    }

    [Theory]
    [InlineData("2024.1")]
    [InlineData("2024.01")]
    [InlineData("2024.12")]
    [InlineData("2049.10")]
    [InlineData("2050.1")]
    public void ValidVersions(string version)
    {
        // arrange
        // act
        var isValidVersion = Transformer.IsValidVersion(version, out _versionRecord);
        
        // assert
        Assert.True(isValidVersion);
    }
    
    [Theory]
    [InlineData("2024.13")]
    [InlineData("2024.001")]
    [InlineData("2024.011")]
    [InlineData("2024.0111")]
    [InlineData("2024.111")]
    [InlineData("20244.111")]
    [InlineData("202.111")]
    [InlineData("2024.0")]
    [InlineData("24.1")]
    [InlineData("24.01")]
    [InlineData("2024.01.01")]
    [InlineData("2024.01.1")]
    [InlineData("2024.1.1")]
    [InlineData("2025.0")]
    [InlineData("24")]
    [InlineData("2024")]
    [InlineData("2024.")]
    [InlineData("2024/")]
    [InlineData("2024-")]
    [InlineData("twenty twenty four point one")]
    [InlineData("twenty.twenty four point one")] // here
    [InlineData("2024!")]
    [InlineData("2024-01")]
    [InlineData("2024/01")]
    [InlineData("2051.10")]
    [InlineData("2051..10")]
    [InlineData("twenty.10")]
    public void InvalidVersions(string version)
    {
        // arrange
        // act
        var isNotValidVersion = Transformer.IsValidVersion(version, out _versionRecord);
        
        // assert
        Assert.False(isNotValidVersion);
    }
    
    private static string ReadEmbeddedResource(string resourceName)
    {
        using (var stream = Assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                throw new InvalidOperationException($"Could not find embedded resource '{resourceName}'");
            }

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}

