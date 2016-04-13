If you just want to run the program  to test the functions you only need put following files in one folder

In WebCrawler/ImgCrawler/ImgCrawler/bin/Debug/
MySql.Data.dll
ImgCrawler.exe
ImgCrawler.exe.config

In WebCrawler/ImageDownload/ImageDownload/bin/Debug/
Dapper.dll
ImageDownload.exe
ImageDownload.exe.config


1.You need have your own mysql database( local/remote)
replace the connection string in ImgCrawler.exe.config with  your own value:

 <add name="mySqlConnString" connectionString="server=*****;uid=***;pwd=***;database=crawlerDb"/>

2. also the connection string in ImageDownload.exe.config with  your own value.


3. <add key="ImgOutDirectory" value="<replace your directory>"/> Change the directory where you want to store your download images.






