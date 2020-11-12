using ghbin.Model;
using System.IO;
using System.Net;

namespace ghbin.Service
{
    public class DownloadService
    {
        private string GhbinPath { get;set; }

        public DownloadService(string ghbinPath) {
            GhbinPath = ghbinPath;
        }
        public void DownloadRelease(Repository repository, Release release) 
        {

            foreach(var asset in release.Assets) {
                using var wc = new WebClient();
                string binDirectoryPath = $"{GhbinPath}/{repository.FullName}/{release.TagName}";
                if(!Directory.Exists(binDirectoryPath)) {
                    Directory.CreateDirectory(binDirectoryPath);
                }
                wc.DownloadFile(asset.BrowserDownloadUrl, $"{binDirectoryPath}/{asset.Name}");
            }
        }
    }
}