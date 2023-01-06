using Abot2.Core;
using Abot2.Crawler;
using Abot2.Poco;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LazyMoon.Service
{
    public class Abot
    {
        private readonly ILogger<Abot> _logger;
        IWebCrawler _crawler;

        public Abot(ILogger<Abot> logger)
        {
            _logger = logger;
            var crawlConfig = new CrawlConfiguration();
            crawlConfig.CrawlTimeoutSeconds = 1000;
            crawlConfig.MaxConcurrentThreads = 10;
            crawlConfig.MaxPagesToCrawl = 10;
            crawlConfig.UserAgentString = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:51.0) Gecko/20100101 Firefox/51.0";
            IWebCrawler crawler = new PoliteWebCrawler(crawlConfig);
            _crawler = crawler;

            // 이벤트 핸들러 셋업
            crawler.PageCrawlStarting += (s, e) =>
            {
                Console.WriteLine();
                _logger.LogInformation("Starting : {0}", e.PageToCrawl);
            };

            crawler.PageCrawlCompleted += (s, e) =>
            {
                CrawledPage pg = e.CrawledPage;

                string fn = pg.Uri.Segments[pg.Uri.Segments.Length - 1];

                _logger.LogInformation("Completed : {0}", pg.Uri.AbsoluteUri);
                _logger.LogInformation(pg.Content.Text);
            };
        }

        public async Task<CrawlResult> Crawl(string Url)
        {
            // 크롤 시작
            string siteUrl = Url;
            Uri uri = new Uri(siteUrl);

           return await _crawler.CrawlAsync(uri);
        }

        public async Task<CrawledPage> PageRequest(string Url)
        {
            var pageRequester = new PageRequester(new CrawlConfiguration(), new WebContentExtractor());
            string siteUrl = Url;
            Uri uri = new Uri(siteUrl);
            return await pageRequester.MakeRequestAsync(new Uri(Url));
        }
    }
}
