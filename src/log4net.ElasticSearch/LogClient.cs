using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch
{   
    /// <summary>
    /// Generic client class for sending http requests to Elasticsearch. Works w/ older .NET versions
    /// </summary>
    public class LogClient
    {
        private readonly HttpWebRequest httpWebRequest;

        public LogClient(ElasticsearchConnection connection)
        {
            ServicePointManager.Expect100Continue = false;
            httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("http://{0}:{1}/{2}/LogEvent", 
                connection.Server, connection.Port, connection.Index));
            
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 10000;
        }

        /// <summary>
        /// Create the new event in Elasticsearch by performing a generic http post
        /// </summary>
        /// <param name="logEvent">LogEvent with parameters filled in</param>
        public void CreateEvent(LogEvent logEvent)
        {
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(logEvent);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                httpWebRequest.BeginGetResponse(new AsyncCallback(FinishWebRequest), httpWebRequest);
            }
        }


        private void FinishWebRequest(IAsyncResult result)
        {
            HttpWebResponse response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;

            if (response.StatusCode != HttpStatusCode.Created)
                throw new InvalidOperationException("Failed to correctly add the event to the Elasticsearch index.");

            response.Close();
        }
    }
}
