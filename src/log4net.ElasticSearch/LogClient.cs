using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using log4net.Config;
using log4net.Core;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch
{   
    /// <summary>
    /// Generic client class for sending http requests to Elasticsearch. Works w/ older .NET versions
    /// </summary>
    public class LogClient
    {
        private readonly HttpWebRequest httpWebRequest;
        private readonly IErrorHandler errorHandler;

        public LogClient(ElasticsearchConnection connection)
        {
           errorHandler = new GenericErrorHandler();

            ServicePointManager.Expect100Continue = false;
            httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("http://{0}:{1}/{2}/logEvent", 
                connection.Server, connection.Port, connection.Index));
            
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Proxy = null;

            // If the user specified a connection timeout, set it here. Default is 200 MS
            if (!string.IsNullOrEmpty(connection.Timeout))
            {
                httpWebRequest.Timeout = Convert.ToInt32(connection.Timeout);
            }
            else
            {
                httpWebRequest.Timeout = 300;
            }
        }

        

        public void CreateEvent(logEvent logEvent)
        {
            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(logEvent);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                }
                RequestState state = new RequestState();
                state.Request = httpWebRequest;

                IAsyncResult result = httpWebRequest.BeginGetResponse(new AsyncCallback(ResponseCallback), state);

                // Timeout comes here
                ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle,
                    new WaitOrTimerCallback(TimeOutCallback), httpWebRequest, httpWebRequest.Timeout, true);
            }
            catch (Exception ex)
            {
                errorHandler.Error("Failed to create entry in Elasticsearch.", ex, ErrorCode.WriteFailure);
            }
        }


        private void TimeOutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                HttpWebRequest request = state as HttpWebRequest;
                if (request != null)
                    request.Abort();
            }
        }

       

        private void ResponseCallback(IAsyncResult result)
        {
            try
            {
                // Get and fill the RequestState
                RequestState state = (RequestState)result.AsyncState;
                HttpWebRequest request = state.Request;
                // End the Asynchronous response and get the actual resonse object
                state.Response = (HttpWebResponse)request.EndGetResponse(result);
                Stream responseStream = state.Response.GetResponseStream();
                state.ResponseStream = responseStream;

                // Begin async reading of the contents
                IAsyncResult readResult = responseStream.BeginRead(state.BufferRead,
                        0, state.BufferSize, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception ex)
            {
                // Error handling
                RequestState state = (RequestState)result.AsyncState;
                if (state.Response != null)
                    state.Response.Close();

                errorHandler.Error("Failed to read response callback.", ex, ErrorCode.WriteFailure);
            }
        }

        private void ReadCallback(IAsyncResult result)
        {
            try
            {
                // Get RequestState
                RequestState state = (RequestState)result.AsyncState;
                // determine how many bytes have been read
                int bytesRead = state.ResponseStream.EndRead(result);

                if (bytesRead > 0) // stream has not reached the end yet
                {
                    // append the read data to the ResponseContent and...
                    state.ResponseContent.Append(Encoding.ASCII.GetString(state.BufferRead, 0, bytesRead));
                    // ...read the next piece of data from the stream
                    state.ResponseStream.BeginRead(state.BufferRead, 0, state.BufferSize,
                        new AsyncCallback(ReadCallback), state);
                }
                else // end of the stream reached
                {
                    if (state.ResponseContent.Length > 0)
                    {
                        // Don't do anything with the content yet
                        state.Response.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // Error handling
                RequestState state = (RequestState)result.AsyncState;
                if (state.Response != null)
                    state.Response.Close();

                errorHandler.Error("Failed to complete read callback.", ex, ErrorCode.WriteFailure);
            }
        }

    }
}
