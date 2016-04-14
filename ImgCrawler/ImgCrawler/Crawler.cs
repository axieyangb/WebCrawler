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
        private Queue<string> cacheQueue = new Queue<string>();
        private HashSet<string> cacheHashSet = new HashSet<string>();
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

        public void SearchInitial(List<string> startList)
        {
            try
            {
                conn.Open();
                conn.Execute("delete  from ImgContent where ImgId>=0");
                conn.Execute("delete  from UrlContent where ListId>=0");
                for (int i = 0; i < startList.Count; i++)
                {
                    conn.Execute("insert into UrlContent(Url,depth) values(@url,0)", new { url = startList[i] });
                }
            }
            catch (Exception)
            {
                //ignore
                throw;
            }
            finally
            {
                conn.Close();
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
                if (conn.State == ConnectionState.Open)
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
                var ret = conn.Query<UrlContent>("select * from UrlContent where Depth=@depth and isVisited=0 limit 1000", new { depth }).ToList();
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

        private bool IsInCache(string url)
        {
            return cacheHashSet.Contains(url);
        }

        public void AddToCache(string url)
        {
            if (cacheQueue.Count >= 10000) cacheHashSet.Remove(cacheQueue.Dequeue());
            cacheQueue.Enqueue(url);
            cacheHashSet.Add(url);
        }
        public bool HtmlUrlInsertIntoDb(List<UrlContent> htmlUrls)
        {
            try
            {
                conn.Open();
                for (int i = 0; i < htmlUrls.Count; i++)
                {
                    if (IsInCache(htmlUrls[i].Url)) continue;
                    int count =
                        conn.Query<int>("select count(*) from UrlContent where Url=@url", new { url = htmlUrls[i].Url })
                            .First();
                    if (count == 0)
                    {
                        conn.Execute(@"insert into UrlContent(Url,Depth) values(@url,@depth)",
                      new { url = htmlUrls[i].Url, depth = htmlUrls[i].Depth });
                    }
                    else
                    {
                        AddToCache(htmlUrls[i].Url);
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
                    int count = conn.Query<int>("select count(*) from ImgContent where ImgUrl=@imgUrl",
                         new { imgUrl = imgUrls[i].ImgUrl }).First();
                    if (count == 0)
                    {
                        conn.Execute(@"insert into ImgContent(ImgUrl,UrlId) values(@url,@urlId)",
                            new { url = imgUrls[i].ImgUrl, urlId = imgUrls[i].UrlId });
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
        public List<ImgContent> FindImgUrl(int htmlUrlId, string htmlContent)
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

        public List<UrlContent> FindHtmlUrl(string htmlContent, int depth, string rootUrl)
        {
            UrlSearchPattern oneSearchPattern = new RegexSearchPattern(htmlContent);
            return oneSearchPattern.GetHtmlUrlList(depth, rootUrl);
        }


    }
}
