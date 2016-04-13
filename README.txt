If you just want to run the program  to test the functions, you only need put following files in one folder

In WebCrawler/ImgCrawler/ImgCrawler/bin/Debug/
MySql.Data.dll
ImgCrawler.exe
ImgCrawler.exe.config

In WebCrawler/ImageDownload/ImageDownload/bin/Debug/
Dapper.dll
ImageDownload.exe
ImageDownload.exe.config


Basically the final directory will looks like this:


MySql.Data.dll
Dapper.dll
ImgCrawler.exe
ImgCrawler.exe.config
ImageDownload.exe
ImageDownload.exe.config



1.You need have your own mysql database( local/remote)
replace the connection string in ImgCrawler.exe.config with  your own value:

 <add name="mySqlConnString" connectionString="server=*****;uid=***;pwd=***;database=crawlerDb"/>

2. also the connection string in ImageDownload.exe.config.


3. <add key="ImgOutDirectory" value="D://GrabPics"/>  (any place you want)
Change the directory path where you prefer to store your download images.

In database crawlerDb

there are two tables 
 1. UrlContent is used to store the html url
 2.ImgContent is used to store the image url

CREATE TABLE `UrlContent` (
   `ListId` bigint(20) NOT NULL AUTO_INCREMENT,
   `Url` varchar(255) DEFAULT NULL,
   `CreateDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
   `Depth` int(11) DEFAULT '0',
   `IsVisited` int(11) DEFAULT '0',
   PRIMARY KEY (`ListId`)
 ) ENGINE=InnoDB AUTO_INCREMENT=64846 DEFAULT CHARSET=latin1

CREATE TABLE `ImgContent` (
   `ImgId` bigint(20) NOT NULL AUTO_INCREMENT,
   `ImgUrl` varchar(255) DEFAULT NULL,
   `UrlId` bigint(20) DEFAULT NULL,
   `IsVisited` int(11) DEFAULT '0',
   `CreateDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
   PRIMARY KEY (`ImgId`),
   KEY `fk_urlId` (`UrlId`),
   CONSTRAINT `fk_urlId` FOREIGN KEY (`UrlId`) REFERENCES `UrlContent` (`ListId`)
 ) ENGINE=InnoDB AUTO_INCREMENT=34917 DEFAULT CHARSET=latin1






