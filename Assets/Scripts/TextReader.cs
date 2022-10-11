using UnityEngine;

public class TextReader
{
    public static string LoadResourceTextfile(string path)
    {
         return System.IO.File.ReadAllText(path);
    }

    public static string LoadResourceTextfileFromStreamingAsset(string path)
    {
        path = System.IO.Path.Combine(Application.streamingAssetsPath, path);
        return LoadResourceTextfile(path);
    }
}



