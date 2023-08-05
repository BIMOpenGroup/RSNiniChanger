namespace RSNiniManager
{
    using System;
    using Spectre.Console;

    public class RSNiniManager
    {
        static public void Main(string[] args)
        {
            try
            {
                string autodeskDataFolder = "C:\\ProgramData\\Autodesk";
                string[] RevitServesFolders = Directory.GetDirectories(autodeskDataFolder, "Revit Server*", SearchOption.TopDirectoryOnly);
                string appdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string fullBackupFolderPath = Path.Combine(appdataFolder, typeof(RSNiniManager).Name);
                string consoleReadLine = "";
                if (Directory.Exists(autodeskDataFolder))
                {
                    while (consoleReadLine != "q")
                    {
                        AnsiConsole.Clear();
                        AnsiConsole.Markup($"[bold {Colors.mainColor}]RSNiniChanger[/]\n");
                        Console.WriteLine("run args:");
                        foreach (var arg in args)
                        {
                            AnsiConsole.Markup($" [{Colors.infoColor}]{arg}[/]");
                        }
                        if (args[0] == "-change")
                        {
                            LaunchModes.ChangeCurrent(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                        }
                        else if (args[0] == "-add")
                        {
                            LaunchModes.AddNewServers(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                        }
                        else if (args[0] == "-remove")
                        {
                            LaunchModes.AddNewServers(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                        }
                        else
                        {
                            Console.WriteLine("\n");
                            var launchMode = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title($"[underline {Colors.mainColor}]Select launch mode[/]\n")
                                    .HighlightStyle(Colors.selectionStyle)
                                    .PageSize(3)
                                    .AddChoices(new[] { "Change current servers", "Add new servers", "Remove servers" })
                               );
                            if (launchMode == "Change current servers")
                            {
                                LaunchModes.ChangeCurrent(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                            }
                            else if (launchMode == "Add new servers")
                            {
                                LaunchModes.AddNewServers(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                            }
                            else if (launchMode == "Remove servers")
                            {
                                LaunchModes.RemoveServers(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                            }
                            else
                            {
                                Console.WriteLine("no one will see this message if the program is working properly");
                            }
                        }
                        Utils.CreateBuckup(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                        AnsiConsole.Markup($"promnt [underline {Colors.attentionColor}]q[/] to exit or [underline {Colors.mainColor}]any button[/] to repeat\n");
                        consoleReadLine = Console.ReadLine();
                    }
                }
                else
                {
                    AnsiConsole.Markup($"[{Colors.attentionColor}]{autodeskDataFolder} - not found[/]");
                    Console.WriteLine("\n");
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                Console.ReadLine();
            }
        }
    }
}
