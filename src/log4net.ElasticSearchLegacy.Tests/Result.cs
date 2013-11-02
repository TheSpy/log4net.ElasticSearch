using System.Collections.Generic;

namespace log4net.ElasticSearchLegacy.Tests
{
    /// <summary>
    /// Basic class to deserialize the ES result into for easier testing
    /// </summary>
    public class Result
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public Shards _shards { get; set; }
        public Hits hits { get; set; }
    }

    public class Shards
    {
        public int total { get; set; }
        public int successful { get; set; }
        public int failed { get; set; }
    }

    public class Source
    {
        public string Id { get; set; }
        public string TimeStamp { get; set; }
        public string Message { get; set; }
        public string MessageObject { get; set; }
        public string Exception { get; set; }
        public string LoggerName { get; set; }
        public string Domain { get; set; }
        public string Identity { get; set; }
        public string Level { get; set; }
        public string ClassName { get; set; }
        public string FileName { get; set; }
        public string LineNumber { get; set; }
        public string FullInfo { get; set; }
        public string MethodName { get; set; }
        public string Fix { get; set; }
        public object Properties { get; set; }
        public string UserName { get; set; }
        public string ThreadName { get; set; }
        public string HostName { get; set; }
    }

    public class Hit
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public double _score { get; set; }
        public Source _source { get; set; }
    }

    public class Hits
    {
        public int total { get; set; }
        public double max_score { get; set; }
        public List<Hit> hits { get; set; }
    }

    
}
