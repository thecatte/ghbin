using System.Collections.Generic;
using ghbin.Model;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Linq;

namespace ghbin.Service
{
    public class ReleaseService
    {
        public async Task<List<Release>> GetReleases(string owner, string repository) {
            using var client = HttpService.GetClient();
            var releases = await client.GetFromJsonAsync<List<Release>>($"https://api.github.com/repos/{owner}/{repository}/releases", HttpService.JsonOptions);

            return releases;
        }

        public async Task<Release> GetLatestRelease(string owner, string repository) {
            using var client = HttpService.GetClient();

            var latestRelease = await client.GetFromJsonAsync<Release>($"https://api.github.com/repos/{owner}/{repository}/releases/latest", HttpService.JsonOptions);

            return latestRelease;
        }

        public async Task<Release> GetRelease(string owner, string repository, string tag) {
            using var client = HttpService.GetClient();

            var release = await client.GetFromJsonAsync<Release>($"https://api.github.com/repos/{owner}/{repository}/releases/tags/{tag}", HttpService.JsonOptions);

            return release;
        }
    }
}