using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIL
{
    class Program
    {
        static void Main(string[] args)
        {
            Interpreter i = new Interpreter("HelloWorld.mil");

            if(i.executionThread.IsAlive)
            {
                i.executionThread.Join();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\nExecution completed, press any key to continue...");
            Console.ReadKey();
        }
    }
}
