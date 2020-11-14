using ghbin.Model;
using System.IO;
using System.Net;

namespace ghbin.Service
{
    public class DownloadService
    {
        private string GhbinPath { get;set; }
        private LoggerService Logger { get;set; }

        public DownloadService(string ghbinPath) {
            GhbinPath = ghbinPath;
            Logger = new LoggerService();
        }

        public void DownloadAsset(string owner, string repository, string tagName, Asset asset) {
            using var wc = new WebClient();
                string binDirectoryPath = $"{GhbinPath}/{owner}/{repository}/{tagName}";
                if(!Directory.Exists(binDirectoryPath)) {
                    Directory.CreateDirectory(binDirectoryPath);
                }
                Logger.Log($"Downloading {asset.Name} into {binDirectoryPath} ...", newLine: false);
                // consider using DownloadFileAsync in the future
                wc.DownloadFile(asset.BrowserDownloadUrl, $"{binDirectoryPath}/{asset.Name}");
                Logger.Log("DONE");
        }

        public void DownloadRelease(string owner, string repository, Release release) {
            foreach(var asset in release.Assets) {
                DownloadAsset(owner, repository, release.TagName, asset);
            }
        }

        public void DownloadRelease(Repository repository, Release release) 
        {
            string[] fullName = repository.FullName.Split('/');
            string owner = fullName[0];
            string repo = fullName[1];

            DownloadRelease(owner, repo, release);
        }
    }
}