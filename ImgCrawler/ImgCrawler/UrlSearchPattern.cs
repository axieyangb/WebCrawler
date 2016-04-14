using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImgCrawler.Model;

namespace ImgCrawler
{
   public abstract class UrlSearchPattern
   {
       protected string HtmlRawContent;
       protected UrlSearchPattern(string rawHtmlData)
       {
           HtmlRawContent = rawHtmlData;
       }

       public abstract List<UrlContent> GetHtmlUrlList(int depth, string rootUrl);
   }
}
