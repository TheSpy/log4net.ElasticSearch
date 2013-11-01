using System;
using System.Collections.Specialized;

namespace log4net.ElasticSearch
{
    /// <summary>
    /// Build a simple ElasticSearch Uri based on the connection string settings
    /// </summary>
    public class ConnectionBuilder
    {
        public static string BuildElsticSearchConnection(string connectionString)
        {
            string returnUrl;

            try
            {
                var builder = new System.Data.Common.DbConnectionStringBuilder();
                builder.ConnectionString = connectionString.Replace("{", "\"").Replace("}", "\"");

                StringDictionary lookup = new StringDictionary();
                foreach (string key in builder.Keys)
                {
                    lookup[key] = Convert.ToString(builder[key]);
                }

                returnUrl = string.Format("http://{0}:{1}/{2}/LogEvent", lookup["Server"], lookup["Port"], lookup["Index"]);

                return returnUrl;
            }
            catch
            {
                throw new InvalidOperationException("Not a valid connection string");
            }
        }
    }
}
