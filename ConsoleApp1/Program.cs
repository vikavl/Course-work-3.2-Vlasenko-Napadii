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

        static (Report greedy, Report bee, Report genetic) RunAlgorithms(InputData inputData, int numberOfIterations = 300, int populationSize = 50)
        {
            var timer = new Stopwatch();
                
            timer.Start();
            var greedyReport = GreedyAlgorithm.Calculate(inputData.n, inputData.a, inputData.b, inputData.B);
            timer.Stop();

            greedyReport.Algorithm = "Greedy algorithm";
            greedyReport.Time = timer.Elapsed.TotalSeconds;

            timer.Restart();
            var beeReport = BeeAlgorithm.Calculate(numberOfIterations, inputData.n, inputData.a.ToArray(), inputData.b.ToArray(), inputData.B);
            timer.Stop();

            beeReport.Algorithm = "Bee algorithm";
            beeReport.Time = timer.Elapsed.TotalSeconds;

            timer.Restart();
            var geneticReport = GeneticAlgorithm.Calculate(inputData.n, inputData.a, inputData.b, inputData.B,
                SelectionType.PartlyRandom, LocalOptimization.Optimize, numberOfIterations, populationSize);
            timer.Stop();
                
            geneticReport.Algorithm = "Genetic algorithm";
            geneticReport.Time = timer.Elapsed.TotalSeconds;
            
            return (greedyReport, beeReport, geneticReport);
        }

        static (Report greedy, Report bee, Report genetic) GetMean(
            List<(Report greedy, Report bee, Report genetic)> data)
        {
            var greedy = data.Select(t => t.greedy);
            var bee = data.Select(t => t.bee);
            var gen = data.Select(a => a.genetic);

            
            var greedyMax = greedy.Select(r => r.ForMax.Result).First();
            var greedyMaxIters = greedy.Select(r => r.ForMax.Iterations).First();
            var greedyMin = greedy.Select(r => r.ForMin.Result).First();
            var greedyMinIters = greedy.Select(r => r.ForMin.Iterations).First();

            var greedyTimes = greedy.Select(r => r.Time);
            var greedyTimeMean = greedyTimes.Sum() / greedyTimes.Count();
            
                
            var beeMaxes = bee.Select(r => r.ForMax.Result);
            var beeMaxesMean = (int) Math.Round(beeMaxes.Sum() / (double) beeMaxes.Count());
            var beeMaxIters = bee.Select(r => r.ForMax.Iterations);
            var beeMaxItersMean = (int) Math.Round(beeMaxIters.Sum() / (double) beeMaxIters.Count());

            var beeTimes = bee.Select(r => r.Time);
            var beeTimeMean = beeTimes.Sum() / beeTimes.Count();

            
            var genMaxes = gen.Select(r => r.ForMax.Result);
            var genMaxesMean = (int) Math.Round(genMaxes.Sum() / (double) genMaxes.Count());
            var genMaxIters = gen.Select(r => r.ForMax.Iterations);
            var genMaxItersMean = (int) Math.Round(genMaxIters.Sum() / (double) genMaxIters.Count());
            var genMines = gen.Select(r => r.ForMin.Result);
            var genMinesMean = (int) Math.Round(genMines.Sum() / (double) genMines.Count());
            var genMinIters = gen.Select(r => r.ForMin.Iterations);
            var genMinItersMean = (int) Math.Round(genMinIters.Sum() / (double) genMinIters.Count());

            var genTimes = gen.Select(r => r.Time);
            var genTimesMean = genTimes.Sum() / genTimes.Count();


            var greedyReport = new Report((greedyMax, greedyMaxIters, null), (greedyMin, greedyMaxIters, null));
            greedyReport.Algorithm = "Greedy algorithm";
            greedyReport.Time = greedyTimeMean;

            var beeReport = new Report((beeMaxesMean, beeMaxItersMean, null), default);
            beeReport.Algorithm = "Bee algorithm";
            beeReport.Time = beeTimeMean;

            var genReport = new Report((genMaxesMean, genMaxItersMean, null), (genMinesMean, genMinItersMean, null));
            genReport.Algorithm = "Genetic algorithm";
            genReport.Time = genTimesMean;

            return (greedyReport, beeReport, genReport);
        }

        static List<int> SizeData { get; set; }
        
        static List<(Report greedy, Report bee, Report genetic)> RunSizeExperiments(int step = 2, int repeats = 3)
        {
            SizeData = new List<int>();
            var inputData = new InputData();
            inputData.ReadInputData();

            var reports = new List<(Report greedy, Report bee, Report genetic)>();

            int nMin = 2, nMax = inputData.n;
            for (inputData.n = 2; inputData.n <= nMax; inputData.n += step)
            {
                SizeData.Add(inputData.n);
                var data = new List<(Report greedy, Report bee, Report genetic)>();
                
                Console.Clear();
                Console.WriteLine($"Running size experiment: {100 * (inputData.n - nMin) / (nMax - nMin)}%");
                for (int j = 0; j < repeats; j++)
                {
                    data.Add(RunAlgorithms(inputData));
                }
                
                reports.Add(GetMean(data));
            }

            return reports;
        }

        static List<int> IterationData { get; set; }
        static List<(Report greedy, Report bee, Report genetic)> RunIterationExperiment(int iterations, int step,
            int numberOfIterations, int repeats = 3)
        {
            IterationData = new List<int>();
            
            var inputData = new InputData();
            inputData.ReadInputData();
            
            var reports = new List<(Report greedy, Report bee, Report genetic)>();
            
            for (int i = 0; i < numberOfIterations; i++, iterations += step)
            {
                IterationData.Add(iterations);
                
                var data = new List<(Report greedy, Report bee, Report genetic)>();

                Console.Clear();
                Console.WriteLine($"Running iteration experiment: {100 * i / numberOfIterations}%");
                for (int j = 0; j < repeats; j++)
                {
                    data.Add(RunAlgorithms(inputData, iterations));
                }

                reports.Add(GetMean(data));
            }

            return reports;
        }

        static List<int> BData { get; set; }
        
        static List<(Report greedy, Report bee, Report genetic)> RunBExperiment(int step = 5, int repeats = 3)
        {
            BData = new List<int>();
            
            var inputData = new InputData();
            inputData.ReadInputData();
            
            var reports = new List<(Report greedy, Report bee, Report genetic)>();

            int BMin = inputData.a.Sum(), BMax = inputData.b.Sum();
            for (inputData.B = inputData.a.Sum(); inputData.B < inputData.b.Sum(); inputData.B += step)
            {
                BData.Add(inputData.B);
                
                Console.Clear();
                Console.WriteLine($"Running B constant experiment: {100 * (inputData.B - BMin) / (BMax - BMin)}%");
                var data = new List<(Report greedy, Report bee, Report genetic)>();

                for (int j = 0; j < repeats; j++)
                {
                    data.Add(RunAlgorithms(inputData));
                }

                reports.Add(GetMean(data));
            }

            return reports;
        }

        static Report GetGenMean(List<Report> reports, string type)
        {
            var maxes = reports.Select(r => r.ForMax.Result);
            var maxesMean = (int) Math.Round(maxes.Sum() / (double) maxes.Count());
            var maxIters = reports.Select(r => r.ForMax.Iterations);
            var maxItersMean = (int) Math.Round(maxIters.Sum() / (double) maxIters.Count());
            var mines = reports.Select(r => r.ForMin.Result);
            var minesMean = (int) Math.Round(mines.Sum() / (double) mines.Count());
            var minIters = reports.Select(r => r.ForMin.Iterations);
            var minItersMean = (int) Math.Round(minIters.Sum() / (double) minIters.Count());
            
            var times = reports.Select(r => r.Time);
            var timesMean = times.Sum() / times.Count();
            
            var report = new Report((maxesMean, maxItersMean, null), (minesMean, minItersMean, null));
            report.Algorithm = type;
            report.Time = timesMean;

            return report;
        }

        static List<int> SelectionIterationData { get; set; }
        static List<(Report outbreeding, Report random, Report bestrandom, Report tournament)> RunSelectionExperiment(
            int iterations, int step, int numberOfIterations, int repeats = 3)
        {
            SelectionIterationData = new List<int>();

            var inputData = new InputData();
            inputData.ReadInputData();
            // inputData.a = new List<int> {-14, 12, 10, 0, -24, 3, 13, 7, -13, 2};
            // inputData.b = new List<int> {10, 19, 22, 11, 1, 10, 13, 19, 1, 20};
            // inputData.n = 10;
            // inputData.B = 70;

            var reports = new List<(Report outbreeding, Report random, Report maxrandom, Report tournament)>();
            int i;
            var timer = new Stopwatch();
            for (i = 0; i < numberOfIterations; i++, iterations += step)
            {
                SelectionIterationData.Add(iterations);
                
                Console.Clear();
                Console.WriteLine($"Running genetic selection experiment: {100 * i / numberOfIterations}%");
                var outbreedings = new List<Report>();
                for (int j = 0; j < repeats; j++)
                {
                    timer.Start();
                    var report = GeneticAlgorithm.Calculate(inputData.n, inputData.a, inputData.b, inputData.B,
                        SelectionType.OutBreeding, LocalOptimization.Optimize, iterations);
                    timer.Stop();
                    report.Time = timer.Elapsed.TotalSeconds;
                    outbreedings.Add(report);
                    timer.Reset();
                }

                var randoms = new List<Report>();
                for (int j = 0; j < repeats; j++)
                {
                    timer.Start();
                    var report = GeneticAlgorithm.Calculate(inputData.n, inputData.a, inputData.b, inputData.B,
                        SelectionType.FullyRandom, LocalOptimization.Optimize, iterations);
                    timer.Stop();
                    report.Time = timer.Elapsed.TotalSeconds;
                    randoms.Add(report);
                    timer.Reset();
                }

                var maxrandoms = new List<Report>();
                for (int j = 0; j < repeats; j++)
                {
                    timer.Start();
                    var report = GeneticAlgorithm.Calculate(inputData.n, inputData.a, inputData.b, inputData.B,
                        SelectionType.PartlyRandom, LocalOptimization.Optimize, iterations);
                    timer.Stop();
                    report.Time = timer.Elapsed.TotalSeconds;
                    maxrandoms.Add(report);
                    timer.Reset();
                }

                var tournaments = new List<Report>();
                for (int j = 0; j < repeats; j++)
                {
                    timer.Start();
                    var report = GeneticAlgorithm.Calculate(inputData.n, inputData.a, inputData.b, inputData.B,
                        SelectionType.Tournament, LocalOptimization.Optimize, iterations);
                    timer.Stop();
                    report.Time = timer.Elapsed.TotalSeconds;
                    tournaments.Add(report);
                    timer.Reset();
                }

                reports.Add((GetGenMean(outbreedings, "Outbreeding"), GetGenMean(randoms, "Random"),
                    GetGenMean(maxrandoms, "Best and random"), GetGenMean(tournaments, "Tournament")));
            }

            return reports;
        }
        
        static List<int> OptimizationIterationData { get; set; }

        static List<(Report optimized, Report non_optimized)> RunOptimizationExperiment(int iterations, int step,
            int numberOfIterations, int repeats = 3)
        {
            OptimizationIterationData = new List<int>();

            var inputData = new InputData();
            inputData.ReadInputData();

            var reports = new List<(Report optimized, Report non_optimized)>();
            int i;
            var timer = new Stopwatch();
            for (i = 0; i < numberOfIterations; i++, iterations += step)
            {
                OptimizationIterationData.Add(iterations);
                
                Console.Clear();
                Console.WriteLine($"Running genetic optimization experiment: {100 * i / numberOfIterations}%");
                var optimized = new List<Report>();
                for (int j = 0; j < repeats; j++)
                {
                    timer.Start();
                    var report = GeneticAlgorithm.Calculate(inputData.n, inputData.a, inputData.b, inputData.B,
                        SelectionType.OutBreeding, LocalOptimization.Optimize, iterations);
                    timer.Stop();
                    report.Time = timer.Elapsed.TotalSeconds;
                    optimized.Add(report);
                    timer.Reset();
                }
                
                var nonOptimized = new List<Report>();
                for (int j = 0; j < repeats; j++)
                {
                    timer.Start();
                    var report = GeneticAlgorithm.Calculate(inputData.n, inputData.a, inputData.b, inputData.B,
                        SelectionType.OutBreeding, LocalOptimization.NotOptimize, iterations);
                    timer.Stop();
                    report.Time = timer.Elapsed.TotalSeconds;
                    nonOptimized.Add(report);
                    timer.Reset();
                }

                reports.Add((GetGenMean(optimized, "Optimized"), GetGenMean(nonOptimized, "Non-optimized")));
            }

            return reports;
        }

        
        // public static int[] a = {-14, 12, 10, 0, -24, 3, 13, 7, -13, 2};
        //
        // public static int[] b = {10, 19, 22, 11, 1, 10, 13, 19, 1, 20};
        //
        // public static int B = 70;


        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH")]
        static void Main(string[] args)
        {
            var inputType = 0;
            (Report greedy, Report bee, Report genetic) reports;
            
            do
            {
                Console.Clear();
                var inputData = new InputData();

                inputType = GetValidInput(
                    "Choose input (1 - random IT; 2 - custom IT; 3 - saved IT; 4 - run experiments 0 - exit)", 
                    value => value >= 0 && value <= 4,
                    "Value does not fit the range.");

                switch (inputType)
                {
                    case 1:
                        inputData.GenerateInputData(GetValidInput(
                            "Enter the number of elements", 
                            value => value > 0 && value % 2 == 0, 
                            "Value must be even."));
                        
                        Console.WriteLine(inputData.ViewInputData());
                        reports = RunAlgorithms(inputData);
                        Console.WriteLine(reports.greedy.ToString() + reports.bee + reports.genetic);

                        if (GetValidInput(
                            "Save input data? (1 - yes; 0 - no)",
                            value => value >= 0 && value < 2,
                            "Value is out of range.") == 1)
                            inputData.SaveInputData();
                        break;
                    case 2:
                        inputData.EnterInputData(GetValidInput(
                            "Enter the number of elements", 
                            value => value > 0 && value % 2 == 0, 
                            "Value must be even."));
                        
                        Console.WriteLine(inputData.ViewInputData());
                        reports = RunAlgorithms(inputData);
                        Console.WriteLine(reports.greedy.ToString() + reports.bee + reports.genetic);

                        if (GetValidInput(
                            "Save input data? (1 - yes; 0 - no)",
                            value => value >= 0 && value < 2,
                            "Value is out of range.") == 1)
                            inputData.SaveInputData();
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
                    case 4:
                        inputData.ReadInputData();
                        if (inputData.n == 0)
                        {
                            Console.WriteLine("No data.");
                            continue;
                        }

                        int iterations = GetValidInput("Enter the number of algorithm iterations to start (possibly 10)");
                        int step = GetValidInput("Enter the step of iterations", value => value > 0,
                            "Step must be bigger then zero.");
                        int numberOfIterations = GetValidInput("Enter the number of experiment iterations (possibly 10)");
                        int BStep = GetValidInput("Enter the step of of B constant (possibly 10)", value => value > 0);
                        
                        var sizeExperimentResults = RunSizeExperiments();
                        
                        var iterationExperimentResult = RunIterationExperiment(iterations, step, numberOfIterations);
                        
                        var BExperimentResult = RunBExperiment(BStep);
                        
                        var selectionExperimentResult =
                            RunSelectionExperiment(iterations, step, numberOfIterations);
                        
                        var optimizationExperimentResult =
                            RunOptimizationExperiment(iterations, step, numberOfIterations);
                        Console.WriteLine();

                        Console.Clear();
                        Console.WriteLine("Writing data to excel file...");

                        inputData.ReadInputData();
                        DataRecorder.Record(inputData, 
                            sizeExperimentResults, SizeData, 
                            iterationExperimentResult, IterationData, 
                            BExperimentResult, BData, 
                            selectionExperimentResult, SelectionIterationData,
                            optimizationExperimentResult, OptimizationIterationData);
                        Console.WriteLine("Done.");
                        break;
                    case 0:
                        return;
                }
                
                Console.Write("Press any key to continue...");
                Console.ReadKey();

            } while (true);
        }
    }
}