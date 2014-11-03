using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace ConsoleApplication1
{
    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("Logging instance: {0}", i.ToString());
                _log.InfoFormat("Logging instance: {0}", i.ToString());
                
                Console.WriteLine("next event");
            }

            Console.ReadKey();
        }
    }
}
