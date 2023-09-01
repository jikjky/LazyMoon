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
        readonly IWebCrawler _crawler;

        public Abot(ILogger<Abot> logger)
        {
            _logger = logger;
            var crawlConfig = new CrawlConfiguration
            {
                CrawlTimeoutSeconds = 1000,
                MaxConcurrentThreads = 10,
                MaxPagesToCrawl = 10,
                UserAgentString = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:51.0) Gecko/20100101 Firefox/51.0"
            };
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

                string fn = pg.Uri.Segments[^1]; // 마지막 문자

                _logger.LogInformation("Completed : {0}", pg.Uri.AbsoluteUri);
                _logger.LogInformation(pg.Content.Text);
            };
        }

        public async Task<CrawlResult> Crawl(string Url)
        {
            // 크롤 시작
            string siteUrl = Url;
            var uri = new Uri(siteUrl);

           return await _crawler.CrawlAsync(uri);
        }

        public async Task<CrawledPage> PageRequest(string Url)
        {
            var pageRequester = new PageRequester(new CrawlConfiguration(), new WebContentExtractor());
            return await pageRequester.MakeRequestAsync(new Uri(Url));
        }
    }
}
