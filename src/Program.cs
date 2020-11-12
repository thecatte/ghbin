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
            string ghbinPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/ghbin";
            var bins = File.ReadAllText($"{ghbinPath}/bins.json");
            var confs = JsonSerializer.Deserialize<Configuration>(bins, new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // repo octocat/hello-world
            // releases yogeshojha/rengine
            // releases bjorn/tiled
            string owner = "bjorn";
            string repository = "tiled";

            var repoService = new RepositoryService();
            var releaseService = new ReleaseService();
            var downloadService = new DownloadService(ghbinPath);

            var repo = await repoService.GetRepository(owner, repository);
            var releases = await releaseService.GetReleases(owner, repository);

            foreach (var release in releases)
            {
                System.Console.WriteLine(release.TagName);
            }

            downloadService.DownloadRelease(repo, releases.First());

            // var latest = releases.OrderByDescending(r => r.PublishedAt).First();
            // foreach (var asset in latest.Assets)
            // {
            //     System.Console.WriteLine(asset.BrowserDownloadUrl);
            // }

            // Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));

            // System.Console.WriteLine(repo.Name);
        }
    }
}
