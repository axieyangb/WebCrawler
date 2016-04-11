

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using ImageDownload;
using ImageDownload.Model;

namespace ImageDownloadTest
{
    [TestFixture]
    public class DownloadTest
    {
        private Downloader oneDownloader;

        [SetUp] 
        public void Init()
        {
            string connectionString = "server=97.99.107.91;uid=axieyangb;pwd=a6163484a;database=crawlerDb";
            DirectoryInfo workDirInfo = new DirectoryInfo(@"D:\GrabPic");
            oneDownloader =new Downloader(connectionString, workDirInfo.FullName);
        }

        [Test]
        public void TestDownload()
        {
            while (true)
            {
                List<ImgContent> imgs = oneDownloader.GetImageUrl();
                if(imgs.Count==0) break;
                for (int i = 0; i < imgs.Count; i++)
                {
                    oneDownloader.DownloadOneImage(imgs[i].ImgUrl);
                }
            }
        }
    }
}
