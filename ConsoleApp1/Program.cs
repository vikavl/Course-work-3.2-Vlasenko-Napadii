using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        // public static int[] a = {-14, 12, 10, 0, -24, 3, 13, 7, -13, 2};
        //
        // public static int[] b = {10, 19, 22, 11, 1, 10, 13, 19, 1, 20};
        //
        // public static int B = 70;

        static List<int> a { get; set; }
        static List<int> b { get; set; }
        
        static int n { get; set; }
        static int B { get; set; }


        static void GenerateInputData()
        {
            var random = new Random();

            n = random.Next(1, 11) * 2;
            
            a = new List<int>();
            b = new List<int>();
            for (int i = 0; i < n; i++)
            {
                a.Add(random.Next(-50, 51));

                var value = 0;
                while ((value = random.Next(1, 100)) < a[i])
                {
                }
                
                b.Add(value);

                B = random.Next(20, 300);
            }
        }
        
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH")]
        static void Main(string[] args)
        {
            GenerateInputData();

            foreach (var a_value in a)
            {
                Console.Write($"{a_value} ");
            }

            Console.WriteLine();

            foreach (var b_value in b)
            {
                Console.Write($"{b_value} ");
            }

            Console.WriteLine();

            Console.WriteLine(n);
            Console.WriteLine(B);

            
            var timer = new Stopwatch();
            
            timer.Start();
            var greedyReport = GreedyAlgorithm.Calculate(10, a, b, B);
            timer.Stop();

            Console.WriteLine(timer.Elapsed.TotalSeconds);
            
            timer.Restart();
            var beeReport = BeeAlgorithm.Calculate(1000, 10, a.ToArray(), b.ToArray(), B);
            timer.Start();

            Console.WriteLine(timer.Elapsed.TotalSeconds);
            
            timer.Restart();
            var geneticReport = GeneticAlgorithm.Calculate(10, a, b, B,
                SelectionType.OutBreeding, LocalOptimization.NotOptimize, 1000, 1000);
            timer.Start();

            Console.WriteLine(timer.Elapsed.TotalSeconds);
        }
    }
}