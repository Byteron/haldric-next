using System.Collections.Generic;
using Godot;

public struct FileData
{
    public string Id;
    public string Path;
    public Resource Data;
}

public static class Loader
{
    public static List<FileData> LoadDir(string path, List<string> extentions, bool loadResource = true)
    {
        return LoadDirectoryData(path, new List<FileData>(), extentions, loadResource);
    }

    private static List<FileData> LoadDirectoryData(string path, List<FileData> directoryData, List<string> extentions, bool loadResource = false)
    {
        var directory = new Directory();

        if (!(directory.Open(path) == Error.Ok))
        {
            GD.Print("Loader: failed to load ", path, ", return [] (open)");
            return directoryData;
        }

        if (!(directory.ListDirBegin(true, true) == Error.Ok))
        {
            GD.Print("Loader: failed to load ", path, ", return [] (list_dir_begin)");
            return directoryData;
        }

        var subPath = "";

        while (true)
        {
            subPath = directory.GetNext();

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
                if (!extentions.Contains(subPath.Extension()))
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

    private static FileData GetFileData(string path, bool loadResource)
    {
        var fileData = new FileData {
            Id = path.GetFile().BaseName(),
            Path = path,
            Data = loadResource ? GD.Load(path) : null
        };

        GD.Print(fileData.Id);

        if (loadResource && fileData.Data == null)
        {
            GD.Print("Loader: could not load file: ", path);
        }

        return fileData;
    }
}