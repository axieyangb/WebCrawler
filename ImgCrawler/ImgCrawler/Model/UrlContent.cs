using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgCrawler.Model
{
    public class UrlContent
    {
        public int ListId { get; set; }
        public string Url { get; set; }
        public int IsVisited { get; set; }
        public int Depth { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
