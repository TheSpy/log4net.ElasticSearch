using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;

namespace ConsoleApplication1
{
    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            for (int i = 0; i < 1000; i++)
            {
                _log.Info("console");
                Thread.Sleep(2000);
            }
            
            
        }
    }
}
