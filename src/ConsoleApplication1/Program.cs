using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace ConsoleApplication1
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
