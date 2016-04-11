using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgCrawler.Model
{
    public class ImgContent
    {
        public int ImgId { get; set; }
        public string ImgUrl { get; set; }
        public int UrlId { get; set; }
        public int IsVisited { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
