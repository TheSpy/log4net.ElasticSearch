using System;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using log4net.Appender;
using log4net.Core;
using log4net.ElasticSearch.Net35.Models;

namespace log4net.ElasticSearch.Net35
{
    public class ElasticSearchAppender : AppenderSkeleton
    {
        public string ConnectionString { get; set; }

        /// <summary>
        /// Add a log to the ElasticSearch repo
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(Core.LoggingEvent loggingEvent)
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                var exception = new InvalidOperationException("Connection string not present.");
                ErrorHandler.Error("Connection string not included in appender.", exception, ErrorCode.GenericFailure);

                return;
            }

            LogEvent logEvent = new LogEvent();

            if (logEvent == null)
            {
                throw new ArgumentNullException("logEvent");
            }

            logEvent.LoggerName = loggingEvent.LoggerName;
            logEvent.Domain = loggingEvent.Domain;
            logEvent.Identity = loggingEvent.Identity;
            logEvent.ThreadName = loggingEvent.ThreadName;
            logEvent.UserName = loggingEvent.UserName;
            logEvent.MessageObject = loggingEvent.MessageObject == null ? "" : loggingEvent.MessageObject.ToString();
            logEvent.TimeStamp = loggingEvent.TimeStamp;
            logEvent.Exception = loggingEvent.ExceptionObject == null ? "" : loggingEvent.MessageObject.ToString();
            logEvent.Message = loggingEvent.RenderedMessage;
            logEvent.Fix = loggingEvent.Fix.ToString();
            logEvent.HostName = Environment.MachineName;

            if (loggingEvent.Level != null)
            {
                logEvent.Level = loggingEvent.Level.DisplayName;
            }

            if (loggingEvent.LocationInformation != null)
            {
                logEvent.ClassName = loggingEvent.LocationInformation.ClassName;
                logEvent.FileName = loggingEvent.LocationInformation.FileName;
                logEvent.LineNumber = loggingEvent.LocationInformation.LineNumber;
                logEvent.FullInfo = loggingEvent.LocationInformation.FullInfo;
                logEvent.MethodName = loggingEvent.LocationInformation.MethodName;
            }

            logEvent.Properties = loggingEvent.Properties.GetKeys().ToDictionary(key => key, key => logEvent.Properties[key].ToString());

            SendError(logEvent);
        }

        /// <summary>
        /// Use basic web request to send an event directly to ElasticSearch
        /// </summary>
        /// <param name="logEvent">LogEvent object</param>
        public void SendError(LogEvent logEvent)
        {
            string url = ConnectionBuilder.BuildElsticSearchConnection(ConnectionString);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            
            String json = JsonConvert.SerializeObject(logEvent);
            StreamWriter writer = new StreamWriter(request.GetRequestStream());
            writer.Write(json);
            writer.Close();

            string responseContent;

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    Stream responseStream = response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException)
            {
                responseContent = string.Format("UNKNOWN");
            }
        }
    }
}
