using ghbin.Model;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace ghbin.Service
{
    public class RepositoryService
    {
        public async Task<Repository> GetRepository(string owner, string repository) {
            using var client = HttpService.GetClient();
            var repo = await client.GetFromJsonAsync<Repository>($"https://api.github.com/repos/{owner}/{repository}", 
                    HttpService.JsonOptions);

            return repo;
        }
    }
}