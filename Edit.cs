using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSNiniChanger
{
    static class Edit
    {
        static public void ChangeCurrent(string[] RevitServesFolders, string autodeskDataFolder, string fullBackupFolderPath)
        {
            Dictionary<string, List<ServerObject>> RevitsAndServers = new Dictionary<string, List<ServerObject>>();
            Dictionary<string, List<string>> NewRevitsAndServers = new Dictionary<string, List<string>>();
            if (Directory.Exists(autodeskDataFolder))
            {
                foreach (var autodeskFolder in RevitServesFolders)
                {
                    RevitsAndServers.Add(autodeskFolder.Replace(autodeskDataFolder, ""), Utils.GetServersFromini(autodeskFolder, true));
                }
                Utils.GetServersFromBackup(RevitServesFolders, autodeskDataFolder, fullBackupFolderPath, ref RevitsAndServers);

                AnsiConsole.Markup("[green]ChangeCurrent mode[/]");
                List<string> revitServFolder = new List<string>();
                Console.WriteLine("\n");

                var selectionPrompt = new MultiSelectionPrompt<string>();
                selectionPrompt.Title("Select revit servers [green]to activate[/]")
                        .NotRequired()
                        .Mode(SelectionMode.Leaf)
                        .WrapAround(true)
                        .PageSize(20)
                        .MoreChoicesText("[grey](Move up and down to reveal more servers)[/]")
                        .InstructionsText(
                            "[grey](Press [blue]<space>[/] to toggle servers, " +
                            "[green]<enter>[/] to accept)[/]");

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

                var selectedRevitServers = AnsiConsole.Prompt(selectionPrompt);

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
            else
            {
                AnsiConsole.Markup($"[red]{autodeskDataFolder} - not found[/]");
                Console.WriteLine("\n");
                Environment.Exit(0);
            }
        }
    }
}
