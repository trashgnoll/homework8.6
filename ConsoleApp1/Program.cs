using System.Diagnostics.Metrics;
using System.IO;

namespace ConsoleApp1
{
    internal class Program
    {
        public static string InputString(string Prompt, bool AllowEmptyInput = false)
        {
            Console.Write(Prompt);
            string? result;
            if (AllowEmptyInput)
                result = Console.ReadLine();
            else
                while ((result = Console.ReadLine()) is null || result == string.Empty)
                    Console.Write(Prompt);
            return (result is not null ? result : string.Empty);
        }
        public static int fileCounter = 0;
        public static int folderCounter = 0;
        public static void ProcessFolder(DirectoryInfo directoryInfo)
        {
            IEnumerable<DirectoryInfo> folders = directoryInfo.GetDirectories();
            if (folders.Any())
            {
                foreach (var di in folders)
                {
                    if (!di.Exists)
                    {
                        Console.WriteLine("Folder " + di.FullName + " not exists");
                        continue;
                    }
                    try { ProcessFolder(di); }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine("Error: no access to folder " + di.FullName);
                    }
                    finally { }
                }
            }
            else
            {
                IEnumerable<FileInfo> files = directoryInfo.GetFiles();
                foreach (var fi in files)
                {
                    if (!fi.Exists)
                    {
                        Console.WriteLine("File " + fi.FullName + " not exists");
                        continue;
                    }
                    try
                    {
                        DateTime fileCreatedDate = File.GetLastWriteTime(fi.FullName);
                        TimeSpan difference = DateTime.Now.Subtract(fileCreatedDate);
                        if (difference.TotalMinutes > 30)
                        {
                            File.Delete(fi.FullName);
                            fileCounter++;
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine("Error: no access to file " + fi.FullName);
                    }
                    finally { }
                }
                files = directoryInfo.GetFiles();
                if (!files.Any()) {
                    DateTime folderCreatedDate = Directory.GetLastWriteTime(directoryInfo.FullName);
                    TimeSpan difference = DateTime.Now.Subtract(folderCreatedDate);
                    if (difference.TotalMinutes > 30)
                    {
                        Directory.Delete(directoryInfo.FullName);
                        folderCounter++;
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            string path = (args.Length == 0 ?
                        InputString("Select folder to delete everything not used more than 30 min: ")
                        : args[0]);
            DirectoryInfo directoryInfo = new(path);
            try
            {
                if (directoryInfo.Exists)
                {
                    ProcessFolder(directoryInfo);
                }
                else
                    throw new Exception("folder not exists");
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            finally { }
            Console.WriteLine("Files deleted: " + fileCounter.ToString() +
                              "\nFolders deleted: " + folderCounter.ToString());
        }
    }
}