using System.Collections.Generic;
using Godot;
using Nakama.TinyJson;

public struct FileData
{
    public string Id;
    public string Path;
    public Resource Data;
}

public static class Loader
{
    public static T LoadJson<T>(string path) where T : class
    {
        var file = new File();
        GD.Print(path);

        if (file.Open(path, File.ModeFlags.Read) != Error.Ok)
        {
            GD.PushError("error reading file");
            return null;
        }

        var jsonString = file.GetAsText();

        file.Close();

        return jsonString.FromJson<T>();
    }

    public static List<FileData> LoadDir(string path, List<string> extensions, bool loadResource = true)
    {
        return LoadDirectoryData(path, new List<FileData>(), extensions, loadResource);
    }

    static List<FileData> LoadDirectoryData(string path, List<FileData> directoryData, List<string> extensions,
        bool loadResource = false)
    {
        var directory = new Directory();

        if (directory.Open(path) != Error.Ok)
        {
            GD.PushWarning("Loader: failed to load " + path + ", return [] (open)");
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
            if (subPath is "." or ".." || subPath.BeginsWith("_"))
            {
                continue;
            }
            else if (subPath == "")
            {
                break;
            }
            else if (directory.CurrentIsDir())
            {
                directoryData = LoadDirectoryData(path, directoryData, extensions, loadResource);
            }
            else
            {
                if (!extensions.Contains(subPath.GetExtension()))
                {
                    continue;
                }

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