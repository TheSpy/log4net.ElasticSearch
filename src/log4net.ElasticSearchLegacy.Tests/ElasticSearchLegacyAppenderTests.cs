using System;
using System.Threading;
using Newtonsoft.Json;
using Xunit;

namespace log4net.ElasticSearchLegacy.Tests
{
    /// <summary>
    /// Use basic 2.0 compatible techniques to log and search ES for our test exception
    /// </summary>
    public class ElasticSearchLegacyAppenderTests : ElasticSearchTestSetup, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ElasticSearchLegacyAppenderTests));

        [Fact]
        public void Can_create_an_event_from_log4net()
        {
            string searchString = "{\"query\":{\"term\":{\"Message\":{\"value\":\"loggingtest\"}}}}";
            string url = String.Format("{0}{1}/LogEvent/_search", TestUrl, TestIndex);

            _log.Error("loggingtest");
            Thread.Sleep(3000);

            var searchResults = SendRequest(url, searchString, "POST");
            var result = JsonConvert.DeserializeObject<Result>(searchResults);
            
            Assert.Equal(1, Convert.ToInt32(result.hits.total));
        }

        public void Dispose()
        {
            DeleteTestIndex();
        }
    }
}
