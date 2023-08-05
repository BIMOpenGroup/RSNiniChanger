using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSNiniManager
{
    static class LaunchModes
    {
        static public void ChangeCurrent(string[] RevitServesFolders, string autodeskDataFolder, string fullBackupFolderPath)
        {
            Dictionary<string, List<ServerObject>> RevitsAndServers = new Dictionary<string, List<ServerObject>>();
            Dictionary<string, List<string>> NewRevitsAndServers = new Dictionary<string, List<string>>();

            foreach (var autodeskFolder in RevitServesFolders)
                {
                    RevitsAndServers.Add(autodeskFolder.Replace(autodeskDataFolder, ""), Utils.GetServersFromini(autodeskFolder, true));
                }
                Utils.GetServersFromBackup(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath, ref RevitsAndServers);

                AnsiConsole.Markup($"[bold {Colors.mainColor}]ChangeCurrent mode[/]");
                List<string> revitServFolder = new List<string>();
                Console.WriteLine("\n");

                var selectionPrompt = new MultiSelectionPrompt<string>();
                selectionPrompt.Title($"[underline {Colors.mainColor}] Select revit servers to activate[/]")
                        .HighlightStyle(Colors.selectionStyle)
                        .NotRequired()
                        .Mode(SelectionMode.Leaf)
                        .WrapAround(true)
                        .PageSize(20)
                        .MoreChoicesText($"[{Colors.infoColor}](Move up and down to reveal more servers)[/]")
                        .InstructionsText(
                            $"[{Colors.infoColor}](Press [{Colors.selectionColor}]<space>[/] to toggle servers, " +
                            $"[{Colors.mainColor}]<enter>[/] to accept)[/]"
                            );

                foreach (KeyValuePair<string, List<ServerObject>> item in RevitsAndServers)
                {
                    List<string> serversName = new List<string>();
                    item.Value.ForEach(serv => serversName.Add(serv.Server));
                    selectionPrompt.AddChoiceGroup(item.Key, serversName);
                    foreach (ServerObject server in item.Value)
                    {
                        if (server.IsMarked)
                        {
                            selectionPrompt.Select(server.Server);
                        }

                    }
                }

                List<string> selectedRevitServers = AnsiConsole.Prompt(selectionPrompt);

                foreach (string server in selectedRevitServers)
                {
                    var RevitName = selectionPrompt.GetParent(server);
                    if (NewRevitsAndServers.ContainsKey(RevitName))
                    {
                        NewRevitsAndServers[RevitName].Add(server);
                    }
                    else
                    {
                        NewRevitsAndServers[RevitName] = new List<string>() { server };
                    }
                }
                foreach (KeyValuePair<string, List<ServerObject>> item in RevitsAndServers)
                {
                    string RevitIniFolerPath = autodeskDataFolder + item.Key;
                    if (NewRevitsAndServers.ContainsKey(item.Key))
                    {
                        Utils.SetServersIni(RevitIniFolerPath, NewRevitsAndServers[item.Key]);
                    }
                    else
                    { 
                        Utils.SetServersIni(RevitIniFolerPath, new List<string>());
                    }
                }
        }

        static public void AddNewServers(string[] RevitServesFolders, string autodeskDataFolder, string fullBackupFolderPath)
        {
            AnsiConsole.Markup($"[bold {Colors.mainColor}]AddNewServers mode[/]");
            Console.WriteLine("\n");
            string revitFolder = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[underline {Colors.mainColor}]Select Revit to add new revitServer[/]\n")
                    .HighlightStyle(Colors.selectionStyle)
                    .PageSize(10)
                    .AddChoices(RevitServesFolders)
                );
            List<string> currentServers = Utils.GetServersFromini(revitFolder);
            AnsiConsole.Markup($"[{Colors.infoColor}]current servers of:[/] {revitFolder}\n");
            currentServers.ForEach(s => AnsiConsole.Markup($" {s}\n"));
            Console.WriteLine("\n");
            AnsiConsole.Markup($"[{Colors.infoColor}]Promt new server name and press [{Colors.selectionColor}]<enter>[/][/]" +
                               $"[{Colors.infoColor}] or press [{Colors.selectionColor}]<esc>[/] to finish adding[/]\n");
            bool isAdd = true;
            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.Escape)
                {
                    break;
                }
                string newServer = Console.ReadLine();
                newServer = cki.KeyChar + newServer;
                if (!string.IsNullOrEmpty(newServer))
                {
                    currentServers.Add(newServer);
                    AnsiConsole.Markup($"server addet: [{Colors.selectionColor}]{newServer}[/]\n");
                }
            }
            Console.WriteLine("\n");
            AnsiConsole.Markup($"[{Colors.mainColor}]Addition completed[/]\n");
            Utils.SetServersIni(revitFolder, currentServers);
        }

        static public void RemoveServers(string[] RevitServesFolders, string autodeskDataFolder, string fullBackupFolderPath)
        {
            AnsiConsole.Markup($"[bold {Colors.mainColor}]RemoveServers mode[/]");
            Console.WriteLine("\n");
            string autodeskfolder = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[underline {Colors.mainColor}]Select Revit to remove revitServer[/]\n")
                    .HighlightStyle(Colors.selectionStyle)
                    .PageSize(10)
                    .AddChoices(RevitServesFolders)
                );
            List<string> currentServers = Utils.GetServersFromini(autodeskfolder);
            string finishRemove = "<finish remove>";
            currentServers.Add(finishRemove);
            string backupFolder = autodeskfolder.Replace(autodeskDataFolder, fullBackupFolderPath);
            List<string> backupServers = Utils.GetServersFromini(backupFolder);

            bool isAdd = true;
            while (true)
            {
                string serverToRemove = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"[underline {Colors.mainColor}]Select Server to remove[/]\n")
                        .HighlightStyle(Colors.selectionStyle)
                        .PageSize(10)
                        .AddChoices(currentServers)
                    );
                if (serverToRemove == finishRemove)
                {
                    currentServers.Remove(finishRemove);
                    break;
                }
                currentServers.Remove(serverToRemove);
                backupServers.Remove(serverToRemove); 
            }

            Utils.SetServersIni(autodeskfolder, currentServers);
            Utils.SetServersIni(backupFolder, backupServers);
        }
    }
}
