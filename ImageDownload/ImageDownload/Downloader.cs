using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly MySqlConnection _conn;
        private readonly DirectoryInfo _imgDir;
        public Downloader(string connString,string imgDir)
        {
             _imgDir = new DirectoryInfo(imgDir);
            try
            {
                _conn = new MySqlConnection(connString);
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public List<ImgContent> GetImageUrl()
        {
            _conn.Open();
            try
            {
                var ret = _conn.Query<ImgContent>("select * from ImgContent where isVisited=0 ORDER BY ImgId limit 100").ToList();
                _conn.Execute("UPDATE ImgContent SET isVisited=1 where isVisited=0 ORDER BY ImgId limit 100");
                return ret;
            }
            catch (Exception ex)
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
                return new List<ImgContent>();
            }
            finally
            {
                _conn.Close();
            }
            
            
        }
        public bool DownloadOneImage(string imgUrl)
        {
            if (imgUrl.ToUpper().EndsWith("GIF")) return true;
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
