using System.Collections.Generic;
using Godot;
using Nakama.TinyJson;

public struct FileData
{
    public string Id { get; set; }
    public string Path { get; set; }
    public Resource Data { get; set; }
}

public static class Loader
{
    public static CT LoadJson<CT>(string path) where CT: class
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

        return JsonParser.FromJson<CT>(jsonString);
    }

    public static List<FileData> LoadDir(string path, List<string> extentions, bool loadResource = true)
    {
        return LoadDirectoryData(path, new List<FileData>(), extentions, loadResource);
    }

     static List<FileData> LoadDirectoryData(string path, List<FileData> directoryData, List<string> extentions, bool loadResource = false)
    {
        var directory = new Directory();

        if (!(directory.Open(path) == Error.Ok))
        {
            GD.PushWarning("Loader: failed to load " + path + ", return [] (open)");
            return directoryData;
        }

        if (!(directory.ListDirBegin() == Error.Ok))
        {
            GD.PushWarning("Loader: failed to load " + path + ", return [] (list_dir_begin)");
            return directoryData;
        }

        while (true)
        {
            string subPath = directory.GetNext();
            if (subPath == "." || subPath == ".." || subPath.BeginsWith("_"))
            {
                continue;
            }
            else if (subPath == "")
            {
                break;
            }
            else if (directory.CurrentIsDir())
            {
                directoryData = LoadDirectoryData(path, directoryData, extentions, loadResource);
            }
            else
            {
                if (!extentions.Contains(subPath.GetExtension()))
                {
                    continue;
                }

                FileData data = GetFileData(directory.GetCurrentDir() + "/" + subPath, loadResource);
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