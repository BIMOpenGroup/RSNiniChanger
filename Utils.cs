using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSNiniChanger
{
    class Utils
    {
        public static void CreateBuckup(string[] RevitServesFolders, string autodeskDataFolder, string fullBackupFolderPath)
        {
            if (!Directory.Exists(fullBackupFolderPath))
            {
                Directory.CreateDirectory(fullBackupFolderPath);
                foreach (var autodeskfolder in RevitServesFolders)
                {
                    CopyDirectory(autodeskfolder, autodeskfolder.Replace(autodeskDataFolder, fullBackupFolderPath), true);
                }
            }
        }

        public static void GetServersFromBackup(string[] RevitServesFolders, 
                                                string autodeskDataFolder, 
                                                string fullBackupFolderPath, 
                                                ref Dictionary<string, List<ServerObject>> RevitsAndServers)
        {
            foreach (var autodeskFolder in RevitServesFolders)
            {
                string backupFolder = autodeskFolder.Replace(autodeskDataFolder, fullBackupFolderPath);
                List<ServerObject> backupini = GetServersFromini(backupFolder, false);

                string folderKey = autodeskFolder.Replace(autodeskDataFolder, "");
                if (RevitsAndServers.ContainsKey(folderKey))
                {
                    foreach (var server in backupini)
                    {
                        if (!RevitsAndServers[folderKey].Contains(server))
                        {
                            RevitsAndServers[folderKey].Add(server);
                        }
                    }
                }
                else
                {
                    RevitsAndServers[folderKey] = backupini;
                }
            }
        }

        public static void WrhiteAndNewLine(string text)
        {
            AnsiConsole.Markup($"[underline red]{text}[/]");
            Console.WriteLine("\n");
        }

        public static List<ServerObject> GetServersFromini(string folder, bool marked)
        {
            var rsnFilePath = Path.Combine(folder, "Config", "RSN.ini");
            var ipServerList = new List<ServerObject>();
            if (File.Exists(rsnFilePath))
            {
                var ipList = File.ReadAllLines(rsnFilePath).ToList();
                ipList.ForEach(serv => ipServerList.Add(new ServerObject(serv, marked)));
            }
            return ipServerList;
        }

        public static List<string> GetServersFromini(string folder)
        {
            var rsnFilePath = Path.Combine(folder, "Config", "RSN.ini");
            var ipServerList = new List<string>();
            if (File.Exists(rsnFilePath))
            {
                ipServerList = File.ReadAllLines(rsnFilePath).ToList();
            }
            return ipServerList;
        }

        public static void SetServersIni(string folder, List<string> newServers)
        {
            var rsnFilePath = Path.Combine(folder, "Config", "RSN.ini");
            if (File.Exists(rsnFilePath))
            {
                File.WriteAllLines(rsnFilePath, newServers);
            }
        }

        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}
