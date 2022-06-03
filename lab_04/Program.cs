using lab_04;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Program
{
    public static void Main()
    {

        IDictionary<string, string[]> types = new Dictionary<string, string[]>();

        types.Add("image", new string[] { ".png", ".webp", ".jpg", ".gif", ".tiff" });
        types.Add("audio", new string[] { ".ogg", ".mp3" });
        types.Add("video", new string[] { ".mkv", ".mp4", ".webm" });
        types.Add("document", new string[] { ".txt", ".doc", ".docx", ".xml", ".xlmx" });

        Console.WriteLine("Podaj ścieżke do folderu");
        string path = Console.ReadLine();
        DirectoryInfo root = new DirectoryInfo(@$"{path}");

        FileInfo[] allFiles = root.GetFiles("*", SearchOption.AllDirectories);
        Array.Sort(allFiles, delegate (FileInfo fi1, FileInfo fi2) { return fi1.Name.CompareTo(fi2.Name); });

        FileInfo[] allFilesSortSize = root.GetFiles("*", SearchOption.AllDirectories);
        Array.Sort(allFilesSortSize, delegate (FileInfo fi1, FileInfo fi2) { return fi2.Length.CompareTo(fi1.Length); });

        FileInfo[] files = root.GetFiles();
        DirectoryInfo[] directories = root.GetDirectories();

        IDictionary<string, long[]> extensions = new Dictionary<string, long[]>();
        IDictionary<string, long[]> groups = new Dictionary<string, long[]>();

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

        foreach(KeyValuePair<string, long[]> extension in extensions)
        {
            bool sucess = false;
            foreach(KeyValuePair<string, string[]> type in types)
            {   
                if (type.Value.Contains(extension.Key))
                {
                    sucess = true;
                    if (groups.ContainsKey(type.Key))
                    {
                        groups[type.Key] = new long[] {
                            groups[type.Key][0] + extension.Value[0],
                            groups[type.Key][1] + extension.Value[1],
                            groups[type.Key][2] < extension.Value[2] ? groups[type.Key][2] : extension.Value[2],
                            groups[type.Key][3] > extension.Value[3] ? groups[type.Key][3] : extension.Value[3]
                        };
                    }
                    else
                    {
                        groups[type.Key] = new long[] { extension.Value[0], extension.Value[1], extension.Value[2], extension.Value[3] };
                    }
                }
                
            }
            if (!sucess)
            {
                if (groups.ContainsKey("Other"))
                {
                    groups["Other"] = new long[] {
                            groups["Other"][0] + extension.Value[0],
                            groups["Other"][1] + extension.Value[1],
                            groups["Other"][2] < extension.Value[2] ? groups["Other"][2] : extension.Value[2],
                            groups["Other"][3] > extension.Value[3] ? groups["Other"][3] : extension.Value[3]
                        };
                }
                else
                {
                    groups["Other"] = new long[] { extension.Value[0], extension.Value[1], extension.Value[2], extension.Value[3] };
                }
                sucess = false;
            }
        }

        Console.WriteLine("Nodes: ");
        Console.WriteLine(String.Format("{0, 12} {1, 7} {2, 12}", "", "[count]", "[total size]"));
        Console.WriteLine(String.Format("{0, 12} {1, 6} {2, 12}", "Directories:", directories.Length, $"{ SizeSuffix(GetDirectioriesSize(allFiles, files)) }"));
        Console.WriteLine(String.Format("{0, 12} {1, 6} {2, 12}", "Files:", files.Length, $"{ SizeSuffix(GetFilesSize(files)) } \n"));

        Console.WriteLine("By Types: ");
        Console.WriteLine(String.Format("{0, 14} {1, 7} {2, 12} {3, 10} {4, 10} {5, 10}", "", "[count]", "[total size]", "[avg size]", "[min size]", "[max size]"));
        foreach (KeyValuePair<string, long[]> group in groups)
        {
            Console.WriteLine(String.Format("{0, 14} {1, 7} {2, 12} {3, 10} {4, 10} {5, 10}", group.Key, group.Value[0], SizeSuffix(group.Value[1]), SizeSuffix(group.Value[1] / group.Value[0]), SizeSuffix(group.Value[2]), SizeSuffix(group.Value[3])));
        }
        Console.WriteLine("\n");
        Console.WriteLine("By extensions:");
        Console.WriteLine(String.Format("{0, 14} {1, 7} {2, 12} {3, 10} {4, 10} {5, 10}", "", "[count]", "[total size]", "[avg size]", "[min size]", "[max size]"));

        foreach (KeyValuePair<string, long[]> extension in extensions)
        {
            Console.WriteLine(String.Format("{0, 14} {1, 7} {2, 12} {3, 10} {4, 10} {5, 10}", extension.Key, extension.Value[0], SizeSuffix(extension.Value[1]), SizeSuffix(extension.Value[1] / extension.Value[0]), SizeSuffix(extension.Value[2]), SizeSuffix(extension.Value[3])));
        }
        Console.WriteLine("\n");
        Console.WriteLine("Ordered by name:");
        Console.WriteLine(String.Format("{0, 40} {1, 10} {2, 60}", "", "[size]", "[path]"));

        foreach (FileInfo file in allFiles)
        {
            Console.WriteLine(String.Format("{0, 40} {1, 10} {2, 60}", file.Name, SizeSuffix(file.Length), file.DirectoryName));
        }

        Console.WriteLine("\n");
        Console.WriteLine("Ordered by sizes:");
        Console.WriteLine(String.Format("{0, 40} {1, 10}", "", "[size]"));

        foreach (FileInfo file in allFilesSortSize)
        {
            Console.WriteLine(String.Format("{0, 40} {1, 10}", file.Name, SizeSuffix(file.Length)));
        }
    }

    static readonly string[] FileSizes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    static string SizeSuffix(Int64 value, int decimalPlaces = 1)
    {
        if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); } 

        int i = 0;
        decimal dValue = (decimal)value;
        while (Math.Round(dValue, decimalPlaces) >= 1000)
        {
            dValue /= 1024;
            i++;
        }

        return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, FileSizes[i]);
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
