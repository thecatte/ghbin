using System;
using System.IO;
using System.Text.Json;
using ghbin.Model;
using ghbin.Service;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ghbin
{
    public class GithubBin
    {
        private string FullGithubBinDirectory
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/" + GithubBinDirectory;
            }
        }
        private readonly string GithubBinDirectory = "ghbin";
        private readonly string GithubBinFile = "bins.json";

        private Configuration Configuration { get; set; }
        private RepositoryService RepositoryService { get; set; }
        private ReleaseService ReleaseService { get; set; }
        private DownloadService DownloadService { get; set; }
        private LoggerService Logger { get; set; }

        public GithubBin()
        {
            RepositoryService = new RepositoryService();
            ReleaseService = new ReleaseService();
            DownloadService = new DownloadService(FullGithubBinDirectory);
            Logger = new LoggerService();

            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            var binFile = File.ReadAllText($"{FullGithubBinDirectory}/{GithubBinFile}");
            Configuration = JsonSerializer.Deserialize<Configuration>(binFile, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        public async Task<List<UpdateInfo>> CheckForUpdates()
        {
            var updateInfos = new List<UpdateInfo>();

            foreach (var bin in Configuration.Bins)
            {
                string[] fullName = bin.FullName.Split('/');
                string owner = fullName[0];
                string repo = fullName[1];

                try
                {
                    var latestRelease = await ReleaseService.GetLatestRelease(owner, repo);
                    var installedRelease = await ReleaseService.GetRelease(owner, repo, bin.Tag);

                    if (latestRelease.PublishedAt > installedRelease.PublishedAt)
                    {
                        updateInfos.Add(new UpdateInfo{
                            FullName = bin.FullName,
                            CurrentTag = installedRelease.TagName,
                            LatestTag = latestRelease.TagName
                        });
                        Logger.Success($"{bin.FullName}: {installedRelease.TagName} -> {latestRelease.TagName}");
                    }
                    else
                    {
                        Logger.Log($"{bin.FullName}: no update");
                    }
                }
                catch
                {
                    Logger.Error($"Couldn't fetch info for {bin.FullName}.");
                }
            }

            return updateInfos;
        }

        public async Task DownloadAll() {
            foreach(var bin in Configuration.Bins) {
                string[] fullName = bin.FullName.Split('/');
                string owner = fullName[0];
                string repo = fullName[1];

                var release = await ReleaseService.GetRelease(owner, repo, bin.Tag);

                DownloadService.DownloadRelease(owner, repo, release);
            }
        }

        public async Task UpgradeAll(List<UpdateInfo> updateInfos = null)
        {
            if(updateInfos == null) {
                updateInfos = await CheckForUpdates();
            }

            foreach (var updateInfo in updateInfos)
            {
                // todo
                // set tag to newest in bins.json
                string[] fullName = updateInfo.FullName.Split('/');
                string owner = fullName[0];
                string repository = fullName[1];

                var release = await ReleaseService.GetRelease(owner, repository, updateInfo.LatestTag);
                DownloadService.DownloadRelease(owner, repository, release);
            }
        }
    }
}