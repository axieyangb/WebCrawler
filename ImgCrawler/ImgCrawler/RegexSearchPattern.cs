using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ImgCrawler.Model;

namespace ImgCrawler
{
    public class RegexSearchPattern : UrlSearchPattern
    {
        public RegexSearchPattern(string rawHtmlData) : base(rawHtmlData)
        {
        }

        public override List<UrlContent> GetHtmlUrlList(int depth, string rootUrl)
        {
            HashSet<UrlContent> ret = new HashSet<UrlContent>();
            MatchCollection matches = Regex.Matches(HtmlRawContent, @"(href="")[-a-zA-Z0-9@:%_\+.~#?&//=]*("")");
            foreach (var oneMatch in matches)
            {
                UrlContent oneUrl = new UrlContent();
                oneUrl.Depth = depth;
                oneUrl.Url = UrlProcess(rootUrl, oneMatch.ToString());
                ret.Add(oneUrl);
            }
            return ret.ToList();
        }

        private string UrlProcess(string rootUrl, string matchUrl)
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
                matchUrl = Regex.Replace(matchUrl, @"(../)", "");
                matchUrl = urlHeader + "/" + matchUrl;
            }
            else if (!matchUrl.StartsWith("http://"))
            {
                List<string> splits = rootUrl.Split('/').ToList();
                splits.RemoveAt(splits.Count - 1);
                matchUrl = String.Join("/", splits) + "/" + matchUrl;
            }
            return matchUrl;
        }
    }
}
