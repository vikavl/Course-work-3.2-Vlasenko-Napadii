using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        // public static int[] a = {-14, 12, 10, 0, -24, 3, 13, 7, -13, 2};
        //
        // public static int[] b = {10, 19, 22, 11, 1, 10, 13, 19, 1, 20};
        //
        // public static int B = 70;
        
        

        public static int GetValidInput(
            string message,
            Predicate<int> predicate = null, 
            string eror = "Wrong input!")
        {
            Console.Write($"{message}: ");
            int value = GetInteger();
            while (!(predicate?.Invoke(value) ?? true))
            {
                Console.Write($"{eror} Try again: ");
                value = GetInteger();
            }
            return value;
        }

        static int GetInteger()
        {
            
            int value = 0;
            while (!int.TryParse(Console.ReadLine(), out value))
            {
                Console.Write("Wrong input! Try again: ");
            }

            return value;
        }

        static (Report greedy, Report bee, Report genetic) RunAlgorithms(InputData inputData, int numberOfIterations = 1000, int populationSize = 50)
        {
            var timer = new Stopwatch();
                
            timer.Start();
            var greedyReport = GreedyAlgorithm.Calculate(inputData.n, inputData.a, inputData.b, inputData.B);
            timer.Stop();

            greedyReport.Algorithm = "Greedy algorithm";
            greedyReport.Time = timer.Elapsed.TotalSeconds;

            timer.Restart();
            var beeReport = BeeAlgorithm.Calculate(numberOfIterations, inputData.n, inputData.a.ToArray(), inputData.b.ToArray(), inputData.B);
            timer.Start();

            beeReport.Algorithm = "Bee algorithm";
            beeReport.Time = timer.Elapsed.TotalSeconds;

            timer.Restart();
            var geneticReport = GeneticAlgorithm.Calculate(inputData.n, inputData.a, inputData.b, inputData.B,
                SelectionType.OutBreeding, LocalOptimization.NotOptimize, numberOfIterations, populationSize);
            timer.Start();
                
            geneticReport.Algorithm = "Genetic algorithm";
            geneticReport.Time = timer.Elapsed.TotalSeconds;
            
            return (greedyReport, beeReport, geneticReport);
        }

        static void RunExperiments(int iterations, int n_min, int n_step, List<int> a_min, int a_step, List<int> b_min, int B_min)
        {
            int i;
            InputData inputData;
            for (i = 0, inputData = new InputData(n_min, a_min, b_min, B_min); i < iterations; 
                i++, inputData = new InputData(n_min += n_step, a_min, b_min, B_min))
            {
                
            }
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH")]
        static void Main(string[] args)
        {
            var inputType = 0;
            (Report greedy, Report bee, Report genetic) reports;
            
            do
            {
                var inputData = new InputData();

                inputType = GetValidInput(
                    "Choose input type (1 - random; 2 - custom; 3 - saved; 0 - exit)", 
                    value => value >= 0 && value <= 3,
                    "Value does not fit the range.");

                switch (inputType)
                {
                    case 1:
                        inputData.GenerateInputData(GetValidInput(
                            "Enter the number of elements", 
                            value => value > 0 && value % 2 == 0, 
                            "Value must be even."));
                        
                        Console.WriteLine(inputData.ViewInputData());

                        if (GetValidInput(
                            "Save data? (1 - yes; 0 - no)",
                            value => value >= 0 && value < 2,
                            "Value is out of range.") == 1)
                            inputData.SaveInputData();
                        
                        reports = RunAlgorithms(inputData);
                        Console.WriteLine(reports.greedy.ToString() + reports.bee + reports.genetic);
                        break;
                    case 2:
                        inputData.EnterInputData(GetValidInput(
                            "Enter the number of elements", 
                            value => value > 0 && value % 2 == 0, 
                            "Value must be even."));
                        
                        Console.WriteLine(inputData.ViewInputData());
                        
                        if (GetValidInput(
                            "Save data? (1 - yes; 0 - no)",
                            value => value >= 0 && value < 2,
                            "Value is out of range.") == 1)
                            inputData.SaveInputData();
                        
                        reports = RunAlgorithms(inputData);
                        Console.WriteLine(reports.greedy.ToString() + reports.bee + reports.genetic);
                        break;
                    case 3:
                        inputData.ReadInputData();

                        if (inputData.n == 0)
                        {
                            Console.WriteLine("No data.");
                            continue;
                        }

                        Console.WriteLine(inputData.ViewInputData());
                        
                        reports = RunAlgorithms(inputData);
                        Console.WriteLine(reports.greedy.ToString() + reports.bee + reports.genetic);
                        break;
                    case 0:
                        return;
                }

            } while (true);
        }
    }
}