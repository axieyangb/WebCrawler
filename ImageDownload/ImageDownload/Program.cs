using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageDownload.Model;

namespace ImageDownload
{
    public class Program
    {
        static void Main(string[] args)
        {
            string connectionString= ConfigurationManager.ConnectionStrings["mySqlConnString"].ConnectionString;
            string outDir = ConfigurationManager.AppSettings["ImgOutDirectory"];
            DirectoryInfo workDirInfo = new DirectoryInfo(outDir);
            Downloader oneDownloader = new Downloader(connectionString, workDirInfo.FullName);
            while (true)
            {
                List<ImgContent> imgs = oneDownloader.GetImageUrl();
                if (imgs.Count == 0) break;
                for (int i=0;i< imgs.Count;i++)
                {
                    oneDownloader.DownloadOneImage(imgs[i].ImgUrl);
                }
            }
        }
    }
}
