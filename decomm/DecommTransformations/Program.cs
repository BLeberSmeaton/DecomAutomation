using DecommTransformations;

var rootPath = args[1];
var version = args[0];

if (args.Length != 2)
{
    Console.WriteLine($"Usage: MyConsoleApp {version}");
    return;
}

var apiRootPath = rootPath + "/Api/Service/";
var solutionFilePath = apiRootPath + "Huxley.API.Full.sln";
var configFilePath = apiRootPath + "packages/repositories.config";
var nugetFilePath = apiRootPath + "build/Nuget.ps1";
var csprojFilePath = apiRootPath + "Hosts/Huxley.API.Version1.WebHost/Huxley.API.Version1.WebHost.csproj";

try
{
    if (!Transformer.IsValidVersion(version, out var versionRecord))
    {
        Console.WriteLine("The version is invalid.");
        Environment.Exit(1);
    }

    if (!Transformer.CheckFileForVersion(File.ReadAllLines(solutionFilePath)) ||
        !Transformer.CheckFileForVersion(File.ReadAllLines(configFilePath)) ||
        !Transformer.CheckFileForVersion(File.ReadAllLines(nugetFilePath)) ||
        !Transformer.CheckFileForVersion(File.ReadAllLines(csprojFilePath)))
    {
        Console.WriteLine("One or more files do not contain the required version.");
        Environment.Exit(1);
    }

    var transformer = new Transformer(versionRecord);

    var transformSolutionFile = transformer.TransformSolutionFile(File.ReadAllLines(solutionFilePath));
    if (transformSolutionFile.Length != 0)
    {
        File.WriteAllText(solutionFilePath, transformSolutionFile.ToString());
        Console.WriteLine("Success, transformed solution file, version removed");
    }
    else
    {
        Environment.Exit(1);
    }

    var transformConfigFile = transformer.TransformConfigFile(File.ReadAllLines(configFilePath));
    if (transformConfigFile.Length != 0)
    {
        File.WriteAllText(configFilePath, transformConfigFile.ToString());
        Console.WriteLine("Success, transformed config file, version removed");
    }
    else
    {
        Environment.Exit(1);
    }

    var transformNugetFile = transformer.TransformNugetFile(File.ReadAllLines(nugetFilePath));
    if (transformNugetFile.Length != 0)
    {
        File.WriteAllText(nugetFilePath, transformNugetFile.ToString());
        Console.WriteLine("Success, transformed nuget file, version removed");
    }
    else
    {
        Environment.Exit(1);
    }

    var transformCsprojFile = transformer.TransformCsprojFile(File.ReadAllLines(configFilePath));
    if (transformConfigFile.Length != 0)
    {
        File.WriteAllText(csprojFilePath, transformCsprojFile.ToString());
        Console.WriteLine("Success, transformed csproj file, version removed");
    }
    else
    {
        Environment.Exit(1);
    }

    Environment.ExitCode = 0;
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    Environment.Exit(1);
}