using System;
using System.IO;
using System.Net;
namespace log4net.ElasticSearchLegacy.Tests
{
    public class ElasticSearchTestSetup
    {
        private const string indexCreationJsonString = "{\"settings\":{\"index\":{\"number_of_replicas\":0,\"number_of_shards\":1}},\"mappings\":{\"LogEvent\":{\"properties\":{\"_id\":{\"type\":\"string\",\"name\":\"_id\"},\"timeStamp\":{\"name\":\"TimeStamp\",\"type\":\"date\"},\"message\":{\"type\":\"string\",\"name\":\"Message\"},\"messageObject\":{\"type\":\"string\",\"name\":\"MessageObject\"},\"exception\":{\"type\":\"string\",\"name\":\"Exception\"},\"loggerName\":{\"type\":\"string\",\"name\":\"LoggerName\"},\"domain\":{\"type\":\"string\",\"name\":\"Domain\"},\"identity\":{\"type\":\"string\",\"name\":\"Identity\"},\"level\":{\"type\":\"string\",\"name\":\"Level\"},\"className\":{\"type\":\"string\",\"name\":\"ClassName\"},\"fileName\":{\"type\":\"string\",\"name\":\"FileName\"},\"name\":{\"type\":\"string\",\"name\":\"Name\"},\"fullInfo\":{\"type\":\"string\",\"name\":\"FullInfo\"},\"methodName\":{\"type\":\"string\",\"name\":\"MethodName\"},\"fix\":{\"type\":\"string\",\"name\":\"Fix\"},\"properties\":{\"type\":\"string\",\"name\":\"Properties\"},\"userName\":{\"type\":\"string\",\"name\":\"UserName\"},\"threadName\":{\"type\":\"string\",\"name\":\"ThreadName\"},\"hostName\":{\"type\":\"string\",\"name\":\"HostName\"}}}}}";
        private const string testUrl = "http://127.0.0.1:9200/";
        private const string testIndex = "log_test";

        public string TestUrl { get; set; }
        public string TestIndex { get; set; }

        public ElasticSearchTestSetup()
        {
            TestUrl = testUrl;
            TestIndex = testIndex;
            SetupTestIndex();
        }

        /// <summary>
        /// Use the defined mapping to setup a local index for testing
        /// </summary>
        public void SetupTestIndex()
        {
            string url = String.Format("{0}{1}", TestUrl, TestIndex);
            SendRequest(url, indexCreationJsonString, "POST");
        }

        /// <summary>
        /// Delete the test index
        /// </summary>
        public void DeleteTestIndex()
        {
            string url = String.Format("{0}{1}", TestUrl, TestIndex);
            SendRequest(url, "", "DELETE");
        }


        /// <summary>
        /// Generic legacy http request sender
        /// </summary>
        /// <param name="url">Full url of request including parameters</param>
        /// <param name="jsonBody">JSON POST body if needed</param>
        /// <param name="verb">HTTP Verb</param>
        public string SendRequest(string url, string jsonBody, string verb)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = verb;
            request.ContentType = "application/json";
            request.Accept = "application/json";

            StreamWriter writer = new StreamWriter(request.GetRequestStream());
            
            if (!String.IsNullOrEmpty(jsonBody))
                writer.Write(jsonBody);
            writer.Close();

            string responseContent;

            using (WebResponse response = request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    responseContent = reader.ReadToEnd();
                }
            }

            return responseContent;
        }
    }


}
