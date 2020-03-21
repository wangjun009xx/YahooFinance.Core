﻿using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using MatthiWare.YahooFinance.Core.Abstractions.History;
using MatthiWare.YahooFinance.Core.Abstractions.Quote;
using MatthiWare.YahooFinance.Core.Abstractions.Search;
using MatthiWare.YahooFinance.Core.History;
using MatthiWare.YahooFinance.Core.Quote;
using MatthiWare.YahooFinance.Core.Search;
using MatthiWare.YahooFinance.Abstractions.Http;
using MatthiWare.YahooFinance.Core.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace MatthiWare.YahooFinance.Core
{
    public class YahooFinanceClient : IYahooFinanceClient
    {
        private const string BaseUrl = "https://query1.finance.yahoo.com/";

        private readonly ILogger<YahooFinanceClient> logger;

        private readonly Lazy<ISearchService> searchServiceLazy;
        private readonly Lazy<IQuoteService> quoteServiceLazy;
        private readonly Lazy<IHistoryService> historyServiceLazy;
        private readonly HttpClient client;
        private readonly IApiClient apiClient;

        private YahooFinanceClient()
        {
            searchServiceLazy = new Lazy<ISearchService>(() => new SearchService(apiClient, logger));
            quoteServiceLazy = new Lazy<IQuoteService>(() => new QuoteService());
            historyServiceLazy = new Lazy<IHistoryService>(() => new HistoryService());
        }

        public YahooFinanceClient(ILogger<YahooFinanceClient> logger)
            : this()
        {
            this.logger = logger ?? NullLogger<YahooFinanceClient>.Instance;

            client = new HttpClient()
            {
                BaseAddress = new Uri(BaseUrl)
            };

            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36");

            apiClient = new ApiClient(client, logger);
        }

        public ISearchService Search => searchServiceLazy.Value;

        public IQuoteService Quote => quoteServiceLazy.Value;

        public IHistoryService History => historyServiceLazy.Value;

        public void Dispose()
        {
            apiClient.Dispose();
        }
    }
}