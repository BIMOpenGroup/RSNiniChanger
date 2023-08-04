namespace RSNiniChanger
{
    using System;
    //using static System.Net.Mime.MediaTypeNames;
    using Spectre.Console;
    using static System.Net.Mime.MediaTypeNames;

    public class Program
    {
        //static string autodeskDataFolder = "C:\\ProgramData\\Autodesk";
        ////static string revitServerFolder = "Revit Server*";
        //static string backupFolder = "RSNiniChanger";
        //static string test_read;

        static public void Main(string[] args)
        {
            try
            {
                string autodeskDataFolder = "C:\\ProgramData\\Autodesk";
                string[] RevitServesFolders = Directory.GetDirectories(autodeskDataFolder, "Revit Server*", SearchOption.TopDirectoryOnly);
                string appdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string fullBackupFolderPath = Path.Combine(appdataFolder, typeof(Program).Namespace);
                string consoleReadLine = "";
                while (consoleReadLine != "q")
                {
                    AnsiConsole.Clear();
                    Console.WriteLine("run args:");
                    foreach (var arg in args)
                    {
                        AnsiConsole.Markup($" [underline grey]{arg}[/]");
                    }
                    if (args[0] == "-change")
                    {
                        Edit.ChangeCurrent(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                    }
                    else if (args[0] == "-add")
                    {
                        AddNewServers(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                    }
                    else
                    {
                        Console.WriteLine("\n");
                        var launchMode = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[underline green]Select launch mode[/]\n")
                                .PageSize(3)
                                .AddChoices(new[] { "Change current servers", "Add new servers" })
                           );
                        if (launchMode == "Change current servers")
                        {
                            Edit.ChangeCurrent(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                        }
                        else if (launchMode == "Add new servers")
                        {
                            AddNewServers(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                        }
                        else
                        {
                            Console.WriteLine("no one will see this message if the program is working properly");
                        }
                    }
                    Utils.CreateBuckup(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                    AnsiConsole.Markup($"promnt [underline red]q[/] to exit or [underline green]any button[/] to repeat\n");
                    consoleReadLine = Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }


        static void AddNewServers(string[] RevitServesFolders, string autodeskDataFolder, string fullBackupFolderPath)
        {
            if (Directory.Exists(autodeskDataFolder))
            {
                string revitFolder = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[underline green]Select Revit to add new revitServer[/]\n")
                        .PageSize(10)
                        .AddChoices(RevitServesFolders)
                   );
                List<string> currentServers = Utils.GetServersFromini(revitFolder);
                currentServers.ForEach(s => AnsiConsole.Markup($" {s}"));
                AnsiConsole.Markup($"\npromt new servers to add\n");
                bool isAdd = true;
                while (isAdd) {
                    string newServer = Console.ReadLine();
                    if (string.IsNullOrEmpty(newServer))
                    {
                        isAdd = false;
                    }
                    else
                    {
                        currentServers.Add(newServer);
                    }
                }
                Utils.SetServersIni(revitFolder, currentServers);
            }
        }
    }
}
