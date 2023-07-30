namespace RSNiniChanger
{
    using System;
    //using static System.Net.Mime.MediaTypeNames;
    using Spectre.Console;
    using static System.Net.Mime.MediaTypeNames;

    public class Program
    {
        static string autodeskDataFolder = "C:\\ProgramData\\Autodesk";
        static string revitServerFolder = "Revit Server*";
        static string backupFolder = "RSNiniChanger";
        static Dictionary<string, List<string>> RevitsAndServers;
        static string test_read;

        static public void Main()
        {
            while (test_read != "q")
            {
                RevitsAndServers = new Dictionary<string, List<string>>();
                string[] RevitServesFolders;
                if (Directory.Exists(autodeskDataFolder))
                {
                    RevitServesFolders = Directory.GetDirectories(autodeskDataFolder, revitServerFolder, SearchOption.TopDirectoryOnly);
                    foreach (var folder in RevitServesFolders)
                    {
                        RevitsAndServers.Add(folder, GetServersFromini(folder));
                    }
                    CreateBuckup(RevitServesFolders);

                    AnsiConsole.Markup("[lime]RSNiniChanger starts[/]");
                    List<string> revitServFolder = new List<string>();
                    Console.WriteLine("\n");
                    var selectFolder = "";

                    var test = new MultiSelectionPrompt<string>();
                    test.Title("What are your [green]favorite fruits[/]?")
                            .NotRequired()
                            .Mode(SelectionMode.Leaf)
                            .WrapAround(true)
                            .PageSize(20)
                            .MoreChoicesText("[grey](Move up and down to reveal more servers)[/]")
                            .InstructionsText(
                                "[grey](Press [blue]<space>[/] to toggle servers, " +
                                "[green]<enter>[/] to accept)[/]");

                    foreach (KeyValuePair<string, List<string>> item in RevitsAndServers)
                    {
                        test.AddChoiceGroup(item.Key, item.Value);
                    }
                    var fruits = AnsiConsole.Prompt(test);

                    // Write the selected fruits to the terminal
                    foreach (string fruit in fruits)
                    {
                        var test2 = test.GetParent(fruit);
                        AnsiConsole.WriteLine(test2);
                        AnsiConsole.WriteLine(fruit);
                    }
                    AnsiConsole.Markup($"promnt [underline red]q[/] to exit\n");
                    test_read = Console.ReadLine();
                    //Console.WriteLine($"Hello, {selectFolder}!");
                }
                else
                {
                    AnsiConsole.Markup($"[red]{autodeskDataFolder} - not found[/]");
                    Console.WriteLine("\n");
                    Environment.Exit(0);
                }
            }
        }

        static void CreateBuckup(string[] RevitServesFolders)
        {
            var appdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var fullBackupFolderPath = Path.Combine(appdataFolder, backupFolder);
            WrhiteAndNewLine(fullBackupFolderPath);
            if (Directory.Exists(fullBackupFolderPath))
            {
                //var RSBackupFolders = Directory.GetDirectories(fullBackupFolderPath, string.Empty, SearchOption.TopDirectoryOnly);
                foreach (var autodeskFolder in RevitServesFolders)
                {
                    string backupFolder = autodeskFolder.Replace(autodeskDataFolder, fullBackupFolderPath);
                    List<string> backupini = GetServersFromini(backupFolder);
                    List<string> revitini = GetServersFromini(autodeskFolder);
                    List<string> test = backupini.Except(revitini).ToList();
                    if (test.Count > 0)
                    {
                        Console.WriteLine(test);
                        AddServersToini(backupFolder, test);
                    }
                    //RevitsAndServers.Add(backupFolder, GetServersFromini(backupFolder));
                }
            }
            else
            {
                Directory.CreateDirectory(fullBackupFolderPath);
                foreach (var autodeskfolder in RevitServesFolders)
                {
                    CopyDirectory(autodeskfolder, autodeskfolder.Replace(autodeskDataFolder, fullBackupFolderPath), true);
                }
            }
        }
    
        static void WrhiteAndNewLine(string text)
        {
            AnsiConsole.Markup($"[underline red]{text}[/]");
            Console.WriteLine("\n");
        }

        static List<string> GetServersFromini(string folder)
        {
            var rsnFilePath = Path.Combine(folder, "Config", "RSN.ini");
            var ipList = new List<string>();
            if (File.Exists(rsnFilePath))
            {
                ipList = File.ReadAllLines(rsnFilePath).ToList();
                //ipList.ToList().ForEach(x => Console.WriteLine(x));
            }
            return ipList;
        }

        static List<string> AddServersToini(string folder, List<string> newServers)
        {
            var rsnFilePath = Path.Combine(folder, "Config", "RSN.ini");
            var ipList = new List<string>();
            if (File.Exists(rsnFilePath))
            {
                File.WriteAllLines(rsnFilePath, newServers);
                //ipList.ToList().ForEach(x => Console.WriteLine(x));
            }
            return ipList;
        }

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
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
