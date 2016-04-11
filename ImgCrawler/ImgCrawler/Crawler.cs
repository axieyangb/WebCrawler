using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Dapper;
using ImgCrawler.Model;
using System.Text.RegularExpressions;
namespace ImgCrawler
{
    public class Crawler
    {
        private MySql.Data.MySqlClient.MySqlConnection conn;

        public Crawler(string connString)
        {
            try
            {
                conn = new MySqlConnection(connString);
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public List<UrlContent> GetHtmlUrl()
        {     
            try
            {
                conn.Open();
                var ret = conn.Query<UrlContent>("select * from UrlContent").ToList();
                return ret;
            }
            catch (Exception e)
            {
                if(conn.State== ConnectionState.Open)
                    conn.Close();
                return new List<UrlContent>();
            }
            finally
            {
                conn.Close();
            }
            
            
        }

        public int GetUnporcessedUrlNumsByDepth(int depth)
        {
            try
            {
                conn.Open();
                var ret = conn.Query<int>("select count(*) from UrlContent where Depth=@depth and isVisited=0", new { depth }).First();
                return ret;
            }
            catch (Exception e)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                return 0;
            }
            finally
            {
                conn.Close();
            }
        }

        public int GetUnprocessedMinimunDepth()
        {
            try
            {
                conn.Open();
                var ret = conn.Query<int>("select min(depth) from UrlContent where isVisited=0").First();
               return ret;
            }
            catch (Exception e)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                return 99999;
            }
            finally
            {
                conn.Close();
            }
        }
        public List<UrlContent> GetHtmlUrlByDepth(int depth)
        {
            try
            {
                conn.Open();
                var ret = conn.Query<UrlContent>("select * from UrlContent where Depth=@depth and isVisited=0 limit 1000",new {depth}).ToList();
                conn.Execute("update UrlContent set isVisited=1 where Depth=@depth and isVisited=0 limit 1000", new { depth });
                return ret;
            }
            catch (Exception e)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                return new List<UrlContent>();
            }
            finally
            {
                conn.Close();
            }
        }
        public List<ImgContent> GetImageUrl()
        {
            conn.Open();
            var ret = conn.Query<ImgContent>("select * from ImgContent").ToList();
            conn.Close();
            return ret;
        }

        public bool HtmlUrlInsertIntoDb(List<UrlContent> htmlUrls)
        {
            try
            {
                conn.Open();
                for (int i = 0; i < htmlUrls.Count; i++)
                {
                    int count =
                        conn.Query<int>("select count(*) from UrlContent where Url=@url", new {url = htmlUrls[i].Url})
                            .First();
                    if (count==0)
                    {
                        conn.Execute(@"insert into UrlContent(Url,Depth) values(@url,@depth)",
                      new { url = htmlUrls[i].Url, depth = htmlUrls[i].Depth });
                    }
                  
                }
                return true;
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public bool ImageUrlInsertIntoDb(List<ImgContent> imgUrls)
        {
            try
            {
                conn.Open();
                for (int i = 0; i < imgUrls.Count; i++)
                {
                   int count= conn.Query<int>("select count(*) from ImgContent where ImgUrl=@imgUrl",
                        new {imgUrl = imgUrls[i].ImgUrl}).First();
                    if (count == 0)
                    {
                        conn.Execute(@"insert into ImgContent(ImgUrl,UrlId) values(@url,@urlId)",
                            new {url = imgUrls[i].ImgUrl, urlId = imgUrls[i].UrlId});
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public int GetConnectionStatus()
        {
            return (int)conn.State;
        }
        public string HttpGetRequest(string url)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                try
                {
                    return client.DownloadString(url);
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        public List<ImgContent> FindImgUrl(int htmlUrlId,string htmlContent)
        {
            HashSet<ImgContent> ret = new HashSet<ImgContent>();
            MatchCollection matches = Regex.Matches(htmlContent,
                @"[-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,4}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?(\.jpg|\.png|\.gif){1}");
            foreach (var oneMatch in matches)
            {
                ImgContent oneUrl = new ImgContent();
                oneUrl.ImgUrl = oneMatch.ToString();
                oneUrl.UrlId = htmlUrlId;
                ret.Add(oneUrl);
            }
            return ret.ToList();
        }

        public List<UrlContent> FindHtmlUrl(string htmlContent,int depth,string rootUrl)
        {
            HashSet<UrlContent> ret = new HashSet<UrlContent>();
            MatchCollection matches = Regex.Matches(htmlContent,@"(href="")[-a-zA-Z0-9@:%_\+.~#?&//=]*("")");
            foreach (var oneMatch in matches)
            {
                UrlContent oneUrl = new UrlContent();
                oneUrl.Depth = depth;
                oneUrl.Url = UrlProcess(rootUrl, oneMatch.ToString());
                ret.Add(oneUrl);
            }
            return ret.ToList();
        }

        public string UrlProcess(string rootUrl, string matchUrl)
        {
            matchUrl = Regex.Replace(matchUrl, @"(href="")|("")", "");
            if (matchUrl.StartsWith("/")) matchUrl = "http://" + new Uri(rootUrl).Host + matchUrl;
            else if (matchUrl.StartsWith("../"))
            {
                List<string> splits = rootUrl.Split('/').ToList();
                splits.RemoveAt(splits.Count - 1);
                MatchCollection upLevel = Regex.Matches(matchUrl, @"(\.\.\/)");
                string urlHeader;
                if (splits.Count - upLevel.Count < 3) urlHeader = "http://" + new Uri(rootUrl).Host;
                else
                {
                    splits.RemoveRange(splits.Count - upLevel.Count, upLevel.Count);
                    urlHeader = String.Join("/", splits);
                }
                matchUrl=Regex.Replace(matchUrl, @"(../)", "");
                matchUrl = urlHeader + "/" + matchUrl;
            }
            else if (!matchUrl.StartsWith("http://"))
            {
                List<string> splits=  rootUrl.Split('/').ToList();
                splits.RemoveAt(splits.Count - 1);
                matchUrl = String.Join("/", splits) + "/" + matchUrl;
            }
            return matchUrl;
        }
    }
}
