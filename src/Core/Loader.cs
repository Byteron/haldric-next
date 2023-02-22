using System.Collections.Generic;
using Godot;
using Nakama.TinyJson;

namespace Haldric;

public struct FileData
{
    public string Id;
    public string Path;
    public Resource? Data;
}

public static class Loader
{
    public static T? LoadJson<T>(string path) where T : class
    {
        GD.Print(path);

        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);

        if (file is null)
        {
            GD.PushWarning($"Error opening file at {path}: {FileAccess.GetOpenError()}");
            return null;
        }

        var jsonString = file.GetAsText();

        return jsonString.FromJson<T>();
    }

    public static List<FileData> LoadDir(string path, List<string> extensions, bool loadResource = true)
    {
        return LoadDirectoryData(path, new List<FileData>(), extensions, loadResource);
    }

    static List<FileData> LoadDirectoryData(string path, List<FileData> directoryData, List<string> extensions,
        bool loadResource = false)
    {
        var directory = DirAccess.Open(path);

        if (directory is null)
        {
            GD.PushWarning($"Error opening directory at {path}: {DirAccess.GetOpenError()}");
            return directoryData;
        }

        if (directory.ListDirBegin() != Error.Ok)
        {
            GD.PushWarning("Loader: failed to load " + path + ", return [] (list_dir_begin)");
            return directoryData;
        }
        
        while (true)
        {
            var subPath = directory.GetNext();
            if (subPath is "." or ".." || subPath.StartsWith("_")) continue;

            if (subPath == "") break;

            if (directory.CurrentIsDir())
            {
                directoryData = LoadDirectoryData(path, directoryData, extensions, loadResource);
            }
            else
            {
                if (!extensions.Contains(subPath.GetExtension())) continue;

                var data = GetFileData(directory.GetCurrentDir() + "/" + subPath, loadResource);
                directoryData.Add(data);
            }
        }

        directory.ListDirEnd();
        return directoryData;
    }

    static FileData GetFileData(string path, bool loadResource)
    {
        var fileData = new FileData
        {
            Id = path.GetFile().GetBaseName(),
            Path = path,
            Data = loadResource ? GD.Load(path) : null
        };

        if (loadResource && fileData.Data == null)
        {
            GD.PushWarning("Loader: could not load file: " + path);
        }

        return fileData;
    }
}