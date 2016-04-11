using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImgCrawler.Model;

namespace ImgCrawler
{
    public class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["mySqlConnString"].ConnectionString;
            CrawlerTask oneTask=new CrawlerTask(connectionString);
            oneTask.StartGrab();
           
           
        }

    }
}
