using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using NUnit.Framework;
using ImgCrawler;
using ImgCrawler.Model;

namespace ImgCrawlerTest
{
    [TestFixture]
    public class CrawlerTest
    {
        private Crawler _oneCrawler;
        [SetUp]
        public void Init()
        {
            string connString = "server=97.99.107.91;uid=axieyangb;pwd=a6163484a;database=crawlerDb";
            _oneCrawler =new Crawler(connString);
        }

        [Test]
        public void TestGetUrl()
        {
          List<ImgContent> imgs= _oneCrawler.GetImageUrl();
            Assert.IsTrue(_oneCrawler.GetConnectionStatus()==0);
            Assert.IsEmpty(imgs);
        }
        [Test]
        public void TestRegexMatch()
        {
           
        }

        [Test]
        public void TestFindHtmlUrl()
        {
            string Url = "http://cl.1024yq.info/thread0806.php?fid=8";
            string webContent = _oneCrawler.HttpGetRequest(Url);
            int depth = 1;
           List<UrlContent> urls= _oneCrawler.FindHtmlUrl(webContent, depth,Url);
            Assert.IsNotEmpty(urls);
        }

        [Test]
        public void HtmlGetTest()
        {
            HttpWebRequest.DefaultCachePolicy=new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create("http://cl.1024yq.info/thread0806.php?fid=8");
            http.UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.0.4) Gecko/20060508 Firefox/1.5.0.4";
            http.AutomaticDecompression=  DecompressionMethods.GZip;
            
            WebResponse response = http.GetResponse();

            var stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string content = sr.ReadToEnd();
        }
    }
}
