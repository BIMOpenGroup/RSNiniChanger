namespace RSNiniManager
{
    using Spectre.Console;
    using System;

    public class RSNiniManager
    {
        static public void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    args = new string[] { "no valid arguments" };
                }
                string autodeskDataFolder = "C:\\ProgramData\\Autodesk";
                List<string> RevitServesFolders = Directory.GetDirectories(autodeskDataFolder, "Revit Server*", SearchOption.TopDirectoryOnly).ToList();
                string appdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string fullBackupFolderPath = Path.Combine(appdataFolder, typeof(RSNiniManager).Name);
                if (Directory.Exists(autodeskDataFolder))
                {
                    Utils.CreateBuckup(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                    while (true)
                    {
                        AnsiConsole.Clear();
                        AnsiConsole.Markup($"[bold {Colors.mainColor}]RSNiniManager[/]\n");
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
                            LaunchModes.AddNewServers(RevitServesFolders);
                        }
                        else if (args[0] == "-remove")
                        {
                            LaunchModes.RemoveServers(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                        }
                        else
                        {
                            Console.WriteLine("\n");
                            var launchMode = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title($"[underline {Colors.mainColor}]Select launch mode[/]\n")
                                    .HighlightStyle(Colors.selectionStyle)
                                    .PageSize(4)
                                    .AddChoices(new[] { "Change current servers", "Add new servers", "Remove servers", "Quit" })
                               );
                            if (launchMode == "Change current servers")
                            {
                                LaunchModes.ChangeCurrent(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                            }
                            else if (launchMode == "Add new servers")
                            {
                                LaunchModes.AddNewServers(RevitServesFolders);
                            }
                            else if (launchMode == "Remove servers")
                            {
                                LaunchModes.RemoveServers(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                            }
                            else if (launchMode == "Quit")
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("no one will see this message if the program is working properly");
                            }
                        }
                        Utils.CreateBuckup(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath);
                        if (args[0] != "no valid arguments")
                        {
                            break;
                        }
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
