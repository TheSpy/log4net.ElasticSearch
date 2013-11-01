using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace log4net.ElasticSearchLegacy.Tests
{
    public class ElasticSearchLegacyAppenderTests
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ElasticSearchLegacyAppenderTests));

        [Fact]
        public void Can_create_an_event_from_log4net()
        {
            _log.Info("loggingtest");
            
            //Thread.Sleep(2000);

            //var searchResults = client.Search<LogEvent>(s => s.Query(q => q.Term(x => x.Message, "loggingtest")));

            //Assert.Equal(1, Convert.ToInt32(searchResults.Hits.Total));

        }
    }
}
