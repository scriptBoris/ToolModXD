using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolModXdLib;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test is running");
            string source = @"D:\RuntimeHDD\DotaXdWorkDir\test.txt";

            var injector = new VersionInjector(source, source);
            injector.Inject();

            foreach(var item in injector.ListSource)
            {
                Console.WriteLine(item.ToString());
            }

            Console.ReadLine();
        }
    }
}
