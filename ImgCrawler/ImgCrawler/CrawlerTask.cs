using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImgCrawler.Model;

namespace ImgCrawler
{
    public class CrawlerTask
    {
        private readonly Crawler _oneProgress;

        public CrawlerTask()
        {
            string connectionString = "server=97.99.107.91;uid=axieyangb;pwd=a6163484a;database=crawlerDb";
            _oneProgress = new Crawler(connectionString);
        }
        public CrawlerTask(String conn)
        {
            string connectionString = conn;
            _oneProgress = new Crawler(connectionString);
        }
        public void StartGrab()
        {
            int depth = _oneProgress.GetUnprocessedMinimunDepth();
            //_oneProgress.SearchInitial(InitialStartSites());
                while (true)
                {
                    List<UrlContent> listOfNotProcessedUrls = _oneProgress.GetHtmlUrlByDepth(depth);
                    if (listOfNotProcessedUrls.Count == 0) break;
                    foreach (var oneUrl in listOfNotProcessedUrls)
                    {
                        string bodyContent = _oneProgress.HttpGetRequest(oneUrl.Url);
                        //image search
                        List<ImgContent> imgsInThisUrl = _oneProgress.FindImgUrl(oneUrl.ListId, bodyContent);
                        //insert into image database
                        _oneProgress.ImageUrlInsertIntoDb(imgsInThisUrl);

                        //html url search
                        List<UrlContent> htmlsInThisUrl = _oneProgress.FindHtmlUrl(bodyContent, depth + 1, oneUrl.Url);
                        //html url insert into database
                        _oneProgress.HtmlUrlInsertIntoDb(htmlsInThisUrl);
                    }
                    if (_oneProgress.GetUnporcessedUrlNumsByDepth(depth) == 0)
                    {
                        depth++;
                    }
                }
            }

        public List<string> InitialStartSites()
        {
            List<string> ret =new List<string>();
            for (int i = 0; i < 100; i++)
            {
                ret.Add("http://cl.1024yq.info/thread0806.php?fid=16&search=&page="+(i+1));
            }
            return ret;
        }
    }
}
