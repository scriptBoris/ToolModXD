using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToolModXdLib;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test is running");
            Console.WriteLine("");
            string path = @"D:\RuntimeHDD\DotaXdWorkDir\85.269";
            string source = @"D:\RuntimeHDD\DotaXdWorkDir\83\UnitUI.slk";
            string target = @"D:\RuntimeHDD\DotaXdWorkDir\85.269\origins\UnitUI.slk";

            int trigger = 1;

            if (trigger == 1 || trigger == 3)
            {
                var injector = new VersionInjectorSlk(source, target);
                injector.EventMessanger += OnMessanger;

                injector.Objectivation();

                Console.WriteLine("\nStarting inject...");
                Thread.Sleep(500);
                injector.Inject();

                Console.WriteLine("\nStarting extract result...");
                Thread.Sleep(500);
                injector.SaveResult(path); 
            }

            if (trigger == 2 || trigger == 3)
            {
                string source2 = @"D:\RuntimeHDD\DotaXdWorkDir\83\commonabilitystrings.txt";
                string target2 = @"D:\RuntimeHDD\DotaXdWorkDir\85.269\origins\commonabilitystrings.txt";
                var injector2 = new VersionInjector(source2, target2);
                injector2.EventMessanger += OnMessanger;

                Console.WriteLine("\nObjectivation...");
                Thread.Sleep(500);
                injector2.Objectivation();

                Console.WriteLine("\nStarting inject...");
                Thread.Sleep(500);
                injector2.Inject();

                Console.WriteLine("\nStarting extract result...");
                Thread.Sleep(500);
                injector2.SaveResult(path); 
            }



            //foreach(var item in injector.ListTarget)
            //{
            //    Console.WriteLine(item.ToString());
            //}

            while (228 == 228)
                Console.ReadLine();
        }

        private static void OnMessanger(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
