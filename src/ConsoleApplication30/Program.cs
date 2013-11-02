using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace ConsoleApplication30
{
    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            _log.Info("console");
        }
    }
}
