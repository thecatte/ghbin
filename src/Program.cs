using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json;
using ghbin.Model;
using ghbin.Service;
using System.Linq;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;

namespace ghbin
{
    class Program
    {
        static void Main(string[] args)
        {
            var githubBin = new GithubBin();

            var app = new CommandLineApplication();
            app.Name = "ghbin";
            app.Description = "Download and manage Github binaries. Check for updates, downgrade, list and more.";
            app.HelpOption("-?|-h|--help");

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 0;
            });

            app.Command("path", (command) =>
            {
                command.Description = "Prints path to dictionary where bins are stored.";

                command.OnExecute(() =>
                {
                    githubBin.PrintPath();
                    return 0;
                });
            });

            app.Command("list", (command) =>
            {
                command.Description = "Lists installed bins.";
                // command.HelpOption("-?|-h|--help");

                command.OnExecute(() =>
                {
                    githubBin.List();
                    return 0;
                });
            });

            app.Command("update", (command) =>
            {
                command.Description = "Fetch updates from Github.";

                command.OnExecute(async () =>
                {
                    await githubBin.CheckForUpdates();
                    return 0;
                });
            });

            app.Command("uninstall", (command) => {
                command.Description = "Uninstall bin and remove all associated files.";
                command.HelpOption("-?|-h|--help");
                
                var whichBinOption = command.Option("-b|--bin", "Which bin to uninstall: \"owner/repository\".", CommandOptionType.SingleValue);

                command.OnExecute(() => {
                    if(whichBinOption.HasValue()) {
                        string whichBinValue = whichBinOption.Value();

                        string[] fullName = whichBinValue.Split('/');
                        githubBin.Uninstall(fullName[0], fullName[1]); 
                    }
                    else {
                        command.ShowHelp();
                    }
                    return 0;
                });
            });

            app.Command("upgrade", (command) =>
            {
                command.Description = "Upgrade bins to newest available version.";
                command.HelpOption("-?|-h|--help");

                var whichBinOption = command.Option("-b|--bin", "Which bin to upgrade: \"all\" or \"owner/repository\".", CommandOptionType.SingleValue);

                command.OnExecute(async () =>
                {
                    if (whichBinOption.HasValue())
                    {
                        string whichBinValue = whichBinOption.Value();

                        if(whichBinValue.Equals("all")) {
                            await githubBin.UpgradeAll();
                        }
                        else
                        {
                            string[] fullName = whichBinValue.Split('/');
                            await githubBin.Upgrade(fullName[0], fullName[1]);
                        }
                    }
                    else
                    {
                        command.ShowHelp();
                    }
                    return 0;
                });
            });

            app.Execute(args);
            //githubBin.Uninstall("atom", "atom");
            //githubBin.Uninstall("bjorn", "tiled");
            // await githubBin.Install("atom", "atom");
            // var updates = await githubBin.CheckForUpdates();
            // await githubBin.DownloadAll();
            // await githubBin.UpgradeAll(updates);
        }
    }
}
