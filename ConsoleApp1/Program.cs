﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToolModXdLib;
using ToolModXdLib.Core;
using ToolModXdLib.Models;

namespace ConsoleApp1
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Test is running");
            Console.WriteLine("");
            string path = @"D:\RuntimeHDD\DotaXdWorkDir\85.269";
            string source = @"D:\RuntimeHDD\DotaXdWorkDir\UnitUI.slk";
            string target = @"D:\RuntimeHDD\DotaXdWorkDir\85.269\origins\UnitUI.slk";

            int trigger = 2;

            //if (trigger == 1 || trigger == 3)
            //{
            //    var injector = new VersionInjectorSlk(source, target);
            //    injector.EventMessanger += OnMessanger;

            //    injector.Objectivation();

            //    Console.WriteLine("\nStarting inject...");
            //    Thread.Sleep(500);
            //    injector.Inject();

            //    Console.WriteLine("\nStarting extract result...");
            //    Thread.Sleep(500);
            //    injector.SaveResult(path); 
            //}

            if (trigger == 2 || trigger == 3)
            {
                string source2 = @"D:\RuntimeHDD\DotaXdWorkDir\83\commonabilitystrings.txt";
                string target2 = @"D:\RuntimeHDD\DotaXdWorkDir\85.269\origins\commonabilitystrings.txt";
                string save2 = @"D:\RuntimeHDD\DotaXdWorkDir\progtest\(listfile)";

                var toolmod = new ToolMod(source);
                toolmod.EventMessanger += OnMessanger;

                toolmod.Init();


                //Thread.Sleep(500);
                //toolmod.LoadTarget(target2);

                //Thread.Sleep(500);
                //toolmod.Inject();

                //Thread.Sleep(500);
                //toolmod.SaveResult(path);

                //Console.WriteLine("start get data for listfile");
                //var listfile = new ListFileInjector();

                //toolmod.GetDataForListfile(listfile);

                //Console.WriteLine("save result listfile");
                //listfile.SaveResult(save2);
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
