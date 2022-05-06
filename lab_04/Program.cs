using lab_04;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Program
{
    public static void Main()
    {

        IDictionary<string, string[]> types = new Dictionary<string, string[]>();

        types.Add("image", new string[] { "png", "webp", "jpg", "gif", "tiff" });
        types.Add("audio", new string[] { "ogg", "mp3" });
        types.Add("video", new string[] { "mkv", "mp4", "webm" });
        types.Add("document", new string[] { "txt", "doc", "docx", "xml", "xlmx" });


        DirectoryInfo root = new DirectoryInfo(@"D:\Program Files\Battlefield V");
        FileInfo[] allFiles = root.GetFiles("*", SearchOption.AllDirectories);
        FileInfo[] files = root.GetFiles();
        DirectoryInfo[] directories = root.GetDirectories();

        Console.WriteLine("Nodes: ");
        Console.WriteLine(String.Format("{0, 12} {1, 7} {2, 12}", "", "[count]", "[total size]"));
        Console.WriteLine(String.Format("{0, 12} {1, 6} {2, 12}", "Directories:", directories.Length, $"{ GetDirectioriesSize(allFiles, files) }B"));
        Console.WriteLine(String.Format("{0, 12} {1, 6} {2, 12}", "Files:", files.Length, $"{ GetFilesSize(files) }B \n"));

        Console.WriteLine("By extensions:");
        Console.WriteLine(String.Format("{0, 14} {1, 7} {2, 12} {3, 10} {4, 10} {5, 10}", "", "[count]", "[total size]", "[avg size]", "[min size]", "[max size]"));

        IDictionary<string, long[]> extensions = new Dictionary<string, long[]>();


        foreach (FileInfo file in allFiles)
        {
            string name = Convert.ToBoolean(file.Extension.Length) ? file.Extension : "other";

            if (extensions.ContainsKey(name))
            {
                extensions[name] = new long[] { 
                    extensions[name][0] + 1,
                    extensions[name][1] + file.Length, 
                    extensions[name][2] < file.Length ? extensions[name][2] : file.Length,
                    extensions[name][3] > file.Length ? extensions[name][3] : file.Length
                };
            }
            else
            {
                extensions[name] = new long[] { 1, file.Length, file.Length, file.Length };
            }
        }

        foreach (KeyValuePair<string, long[]> extension in extensions)
        {
            Console.WriteLine(String.Format("{0, 14} {1, 7} {2, 12} {3, 10} {4, 10} {5, 10}", extension.Key, extension.Value[0], extension.Value[1], extension.Value[1] / extension.Value[0], extension.Value[2], extension.Value[3]));
        }

    }
    public static long GetFilesSize(FileInfo[] files)
    {
        long size = 0;
        
        foreach (FileInfo file in files)
        {
            size += file.Length;
        }

        return size;
    }
    public static long GetDirectioriesSize(FileInfo[] allFiles, FileInfo[] files)
    {
        return GetFilesSize(allFiles) - GetFilesSize(files);
    }


}
