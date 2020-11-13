using System;
using System.IO;
using System.Text.Json;
using ghbin.Model;
using ghbin.Service;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

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
            // TODO First-run config creation.
            // Now it can easily crash if the file is missing.
            var binFile = File.ReadAllText($"{FullGithubBinDirectory}/{GithubBinFile}");
            Configuration = JsonSerializer.Deserialize<Configuration>(binFile, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        private void SaveConfiguration()
        {
            string configuration = JsonSerializer.Serialize<Configuration>(Configuration, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText($"{FullGithubBinDirectory}/{GithubBinFile}", configuration);
        }

        public async Task Install(string owner, string repository)
        {
            if (Configuration.Bins.Any(b => b.FullName.Equals($"{owner}/{repository}")))
            {
                Logger.Warn($"{owner}/{repository} already installed.");
                return;
            }

            try
            {
                Logger.Log($"Installing {owner}/{repository}... ", newLine: false);
                var latestRelease = await ReleaseService.GetLatestRelease(owner, repository);
                Logger.Log($"{latestRelease.TagName}", newLine: false);
                Configuration.Bins.Add(new Bin
                {
                    FullName = $"{owner}/{repository}",
                    Tag = latestRelease.TagName
                });

                SaveConfiguration();
                Logger.Log($" DONE");
            }
            catch (Exception ex)
            {
                Logger.Error($"{owner}/{repository} failed to install. ({ex.Message})");
            }
        }

        public void Uninstall(string owner, string repository)
        {
            var binToUninstall = Configuration.Bins.FirstOrDefault(b => b.FullName.Equals($"{owner}/{repository}"));

            if (binToUninstall != null)
            {
                Logger.Log($"Uninstalling {owner}/{repository} from {FullGithubBinDirectory}/{owner}/{repository} ...", newLine: false);
                Configuration.Bins.Remove(binToUninstall);

                var dir = new DirectoryInfo($"{FullGithubBinDirectory}/{owner}/{repository}");
                if (dir.Exists)
                {
                    foreach (var item in dir.GetDirectories())
                    {
                        Directory.Delete(item.FullName, true);
                    }
                }
                else
                {
                    Logger.Warn("directory not found. Skipping. ", newLine: false);
                }
                SaveConfiguration();
                Logger.Log($"DONE");
            }
            else
            {
                Logger.Error($"{owner}/{repository} not found.");
            }
        }

        public void List()
        {
            foreach (var bin in Configuration.Bins)
            {
                Logger.Log($"{bin.FullName}:{bin.Tag}");
            }
        }

        public void PrintPath()
        {
            Logger.Log($"{FullGithubBinDirectory}");
        }

        public async Task<List<UpdateInfo>> CheckForUpdates(string onlyOwner = null, string onlyRepository = null)
        {
            var updateInfos = new List<UpdateInfo>();

            var binsToCheck = Configuration.Bins;
            if (onlyOwner != null && onlyRepository != null)
            {
                binsToCheck = Configuration.Bins.Where(b => b.FullName.Equals($"{onlyOwner}/{onlyRepository}")).ToList();
            }

            foreach (var bin in binsToCheck)
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
                        updateInfos.Add(new UpdateInfo
                        {
                            FullName = bin.FullName,
                            CurrentTag = installedRelease.TagName,
                            LatestTag = latestRelease.TagName
                        });
                        Logger.Success($"{bin.FullName}: {installedRelease.TagName} -> {latestRelease.TagName}");
                    }
                    else
                    {
                        Logger.Log($"{bin.FullName}: {installedRelease.TagName} up-to-date");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"{bin.FullName}: error while fetching info. ({ex.Message})");
                }
            }

            return updateInfos;
        }

        public async Task DownloadAll()
        {
            foreach (var bin in Configuration.Bins)
            {
                string[] fullName = bin.FullName.Split('/');
                string owner = fullName[0];
                string repo = fullName[1];

                var release = await ReleaseService.GetRelease(owner, repo, bin.Tag);

                DownloadService.DownloadRelease(owner, repo, release);
            }
        }

        public async Task Upgrade(string owner, string repo)
        {
            var updateInfos = await CheckForUpdates(owner, repo);
            await UpgradeAll(updateInfos);
        }

        public async Task UpgradeAll(List<UpdateInfo> updateInfos = null)
        {
            if (updateInfos == null)
            {
                updateInfos = await CheckForUpdates();
            }

            Logger.Log($"{updateInfos.Count} bins will be upgraded.");

            if (updateInfos.Count == 0)
            {
                return;
            }

            foreach (var updateInfo in updateInfos)
            {
                string[] fullName = updateInfo.FullName.Split('/');
                string owner = fullName[0];
                string repository = fullName[1];

                var release = await ReleaseService.GetRelease(owner, repository, updateInfo.LatestTag);
                DownloadService.DownloadRelease(owner, repository, release);

                var bin = Configuration.Bins.FirstOrDefault(b => b.FullName.Equals(updateInfo.FullName));
                bin.Tag = updateInfo.LatestTag;
            }

            SaveConfiguration();
        }
    }
}