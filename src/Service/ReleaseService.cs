using System.Collections.Generic;
using ghbin.Model;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ghbin.Service
{
    public class ReleaseService
    {
        public async Task<List<Release>> GetReleases(string owner, string repository) {
            using var client = HttpService.GetClient();
            var releases = await client.GetFromJsonAsync<List<Release>>($"https://api.github.com/repos/{owner}/{repository}/releases", HttpService.JsonOptions);

            return releases;
        }
    }
}