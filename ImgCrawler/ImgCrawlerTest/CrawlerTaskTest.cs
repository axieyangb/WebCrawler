using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImgCrawler;
using NUnit.Framework.Internal;
using NUnit.Framework;
namespace ImgCrawlerTest
{
    [TestFixture]
    public class CrawlerTaskTest
    {
        private CrawlerTask oneTask;
        [SetUp]
        public void Init()
        {
            oneTask=new CrawlerTask();
        }

        [Test]
        public void TestGrabUrl()
        {
            
            oneTask.StartGrab();
        }
    }
}
