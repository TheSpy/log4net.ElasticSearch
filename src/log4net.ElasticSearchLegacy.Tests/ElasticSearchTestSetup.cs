using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace log4net.ElasticSearchLegacy.Tests
{
    public class ElasticSearchTestSetup
    {
        private const string indexCreationJsonString = "{\"settings\":{\"index\":{\"number_of_replicas\":0,\"number_of_shards\":1}},\"mappings\":{\"LogEvent\":{\"properties\":{\"_id\":{\"type\":\"string\",\"name\":\"_id\"},\"timeStamp\":{\"name\":\"TimeStamp\",\"type\":\"date\"},\"message\":{\"type\":\"string\",\"name\":\"Message\"},\"messageObject\":{\"type\":\"string\",\"name\":\"MessageObject\"},\"exception\":{\"type\":\"string\",\"name\":\"Exception\"},\"loggerName\":{\"type\":\"string\",\"name\":\"LoggerName\"},\"domain\":{\"type\":\"string\",\"name\":\"Domain\"},\"identity\":{\"type\":\"string\",\"name\":\"Identity\"},\"level\":{\"type\":\"string\",\"name\":\"Level\"},\"className\":{\"type\":\"string\",\"name\":\"ClassName\"},\"fileName\":{\"type\":\"string\",\"name\":\"FileName\"},\"name\":{\"type\":\"string\",\"name\":\"Name\"},\"fullInfo\":{\"type\":\"string\",\"name\":\"FullInfo\"},\"methodName\":{\"type\":\"string\",\"name\":\"MethodName\"},\"fix\":{\"type\":\"string\",\"name\":\"Fix\"},\"properties\":{\"type\":\"string\",\"name\":\"Properties\"},\"userName\":{\"type\":\"string\",\"name\":\"UserName\"},\"threadName\":{\"type\":\"string\",\"name\":\"ThreadName\"},\"hostName\":{\"type\":\"string\",\"name\":\"HostName\"}}}}}";

        public ElasticSearchTestSetup()
        {
            SetupTestIndex();
        }


        public void SetupTestIndex()
        {
            string url = "http://127.0.0.1:9200/log_test";
            SendRequest(url, indexCreationJsonString, "POST");
        }

        public void DeleteTestIndex()
        {
            string url = "http://jtoto-win81:9200/log_test";
            SendRequest(url, "", "DELETE");
        }



        public void SendRequest(string url, string jsonBody, string verb)
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
            
        }
    }


}
