using DecommTransformations.Versioning;

namespace DecommTransformations;

public class Transformer
{
    private static VersionRecords _versionRecord;
    private static string _searchVersionXSeparator;
    private static string _searchVersionPeriodSeparator;

    public Transformer(VersionRecords versionRecord)
    {
        _versionRecord = versionRecord;
    }

    public static bool CheckFileForVersion(string[] file)
    {
        return file.Any(line => line.Contains(_searchVersionXSeparator)) ||
               file.Any(line => line.Contains(_searchVersionPeriodSeparator));
    }

    public string[] TransformSolutionFile(string[] file)
    {
        if (file.Length == 0)
        {
            return Array.Empty<string>();
        }

        var searchProjectName = $"Huxley.API.Huxley{_searchVersionXSeparator}.Cloud.csproj";

        var line = file.FirstOrDefault(l => l.Contains(searchProjectName));

        if (line == null)
        {
            return Array.Empty<string>();
        }

        var guid = line.Split(", ").Last().Replace("\"", string.Empty);

        var filteredList = file.Where(l => !l.Contains(guid)).ToList();

        for (int i = 0; i < filteredList.Count - 1; i++)
        {
            if (filteredList[i] == "EndProject" && filteredList[i + 1] == "EndProject")
            {
                filteredList.RemoveAt(i);
            }
        }

        return filteredList.ToArray();
    }

    public string[] TransformConfigFile(string[] file)
    {
        if (file.Length == 0)
        {
            return Array.Empty<string>();
        }

        var searchVersion = $"Huxley.API.Huxley{_searchVersionXSeparator}.Cloud";
        var line = file.FirstOrDefault(l => l.Contains(searchVersion));

        if (line == null)
        {
            return Array.Empty<string>();
        }

        var filteredList = file.Where(x => x != line).ToArray();
        return filteredList;
    }

    public string[] TransformNugetFile(string[] file)
    {
        if (file.Length == 0)
        {
            return Array.Empty<string>();
        }

        return file.Where(l => !l.Contains(_searchVersionPeriodSeparator)).ToArray();
    }

    public string[] TransformCsprojFile(string[] file)
    {
        if (file.Length == 0)
        {
            return Array.Empty<string>();
        }

        var list = file.ToList();
        var searchVersion = $"{_versionRecord.MajorVersion}X{_versionRecord.MinorVersion}";

        for (int i = 0; i < list.Count - 1; i++)
        {
            if (list[i].Contains($"Huxley.API.Huxley{searchVersion}.Cloud.csproj"))
            {
                list.RemoveRange(i, 4);
                break;
            }
        }

        for (int i = 0; i < list.Count - 1; i++)
        {
            if (list[i].Contains($"HUX{searchVersion}PAK") || list[i].Contains($"HUX{searchVersion}DOM") ||
                list[i].Contains($"$(TargetDir)Huxley{searchVersion}"))
            {
                list.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < list.Count - 1; i++)
        {
            if (list[i].Contains($"(HUX{searchVersion}->'%(FullPath)')"))
            {
                list.RemoveRange(i, 5);
                list.RemoveAt(i);
                break;
            }
        }

        return list.ToArray();
    }

    public static bool IsValidVersion(string version, out VersionRecords versionRecord)
    {
        versionRecord = null!;

        var startYear = 2023;
        var endYear = 2050;

        var versionSplit = version.Split('.');

        if (versionSplit.Length != 2)
            return false;

        var majorString = versionSplit[0];
        var minorString = versionSplit[1];
        
        if (minorString.Length < 1 || minorString.Length > 2) 
            return false;

        if (!minorString.StartsWith('0') && !minorString.StartsWith('1'))
            return false;
        
        if (!int.TryParse(majorString, out int major) || !int.TryParse(minorString, out int minor))
            return false;
        
        versionRecord = new VersionRecords(major, minor);
        _searchVersionPeriodSeparator = $"{versionRecord.MajorVersion}.{versionRecord.MinorVersion}";
        _searchVersionXSeparator = $"{versionRecord.MajorVersion}X{versionRecord.MinorVersion}";
        
        return (major >= startYear && major <= endYear) && (minor >= 1 && minor <= 12);
    }
}