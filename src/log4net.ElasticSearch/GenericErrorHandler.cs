using System;
using System.Diagnostics;
using System.Text;
using log4net.Core;

namespace log4net.ElasticSearch
{
    public class GenericErrorHandler : IErrorHandler
    {
        public void Error(string message, Exception e, ErrorCode errorCode)
        {
            WriteToEventLog(message, e, errorCode);
        }

        public void Error(string message, Exception e)
        {
            WriteToEventLog(message, e, ErrorCode.GenericFailure);
        }

        public void Error(string message)
        {
            WriteToEventLog(message, new Exception(), ErrorCode.GenericFailure);
        }

        /// <summary>
        /// If we failed to write to the ES event log, let the Windows event log know
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="e">System exception</param>
        /// <param name="errorCode">log4net Error Code</param>
        private void WriteToEventLog(string message, Exception e, ErrorCode errorCode)
        {
            string sSource;
            string sLog;
            var sEvent = new StringBuilder();

            sSource = "log4net.Elasticsearch";
            sLog = "Application";
            sEvent.Append(message);
            sEvent.AppendLine();

            sEvent.Append("log4net Error Code: " + errorCode);
            sEvent.AppendLine();

            if (e != null)
            {
                sEvent.Append(e.Message);
                sEvent.AppendLine();
                sEvent.Append(e.InnerException);
            }

            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, sLog);

            EventLog.WriteEntry(sSource, sEvent.ToString());
            EventLog.WriteEntry(sSource, sEvent.ToString(),
                EventLogEntryType.Error);
        }
    }
}
