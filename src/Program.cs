using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json;
using ghbin.Model;
using ghbin.Service;
using System.Linq;
using System.IO;

namespace ghbin
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var githubBin = new GithubBin();
            //githubBin.Uninstall("atom", "atom");
            githubBin.Uninstall("bjorn", "tiled");
            // await githubBin.Install("atom", "atom");
            // var updates = await githubBin.CheckForUpdates();
            // await githubBin.DownloadAll();
            // await githubBin.UpgradeAll(updates);
        }
    }
}
