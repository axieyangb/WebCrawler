using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using ImageDownload.Model;
using MySql.Data.MySqlClient;

namespace ImageDownload
{
    
    public  class Downloader
    {
        private readonly MySqlConnection conn;
        private DirectoryInfo _imgDir;
        public Downloader(string connString,string imgDir)
        {
             _imgDir = new DirectoryInfo(imgDir);
            try
            {
                conn = new MySqlConnection(connString);
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public List<ImgContent> GetImageUrl()
        {
            conn.Open();
            try
            {
                var ret = conn.Query<ImgContent>("select * from ImgContent where isVisited=0 ORDER BY ImgId limit 100").ToList();
                conn.Execute("UPDATE ImgContent SET isVisited=1 where isVisited=0 ORDER BY ImgId limit 100");
                return ret;
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                return new List<ImgContent>();
            }
            finally
            {
                conn.Close();
            }
            
            
        }
        public bool DownloadOneImage(string imgUrl)
        {
            string[] splits = imgUrl.Split('/');
            string fileName = splits[splits.Length - 1];
            string[] fsplits = fileName.Split('.');
            if(fsplits.Length>=2)
            fsplits[fsplits.Length - 2] += "_"+DateTime.Now.ToFileTime();
            fileName = String.Join(".", fsplits);
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile(imgUrl, Path.Combine(_imgDir.FullName, fileName));
                }
                catch (Exception e)
                {
                    //ignore
                    //throw new Exception(e.ToString());
                }
                
            }
            return true;
        }
    }
}
