using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing.Chart.ChartEx;
using OfficeOpenXml.Style;

namespace ConsoleApp1
{
    public static class DataRecorder
    {
        public static void Record(
            InputData inputData,
            List<(Report greedy, Report bee, Report genetic)> sizeExperimentResults, List<int> sizeData,
            List<(Report greedy, Report bee, Report genetic)> iterationExperimentResult, List<int> iterData,
            List<(Report greedy, Report bee, Report genetic)> BExperimentResult, List<int> BData,
            List<(Report outbreeding, Report random, Report bestrandom, Report tournament)> selectionExperimentResult, 
            List<int> selectionIterData,
            List<(Report optimized, Report non_optimized)> optimizationExperimentResult, List<int> optimizationIterData)
        {
            using (var excel = new ExcelPackage())
            {
                var inputDataSheet = excel.Workbook.Worksheets.Add("Input data");
                var sizeExperimentSheet = excel.Workbook.Worksheets.Add("IT size experiment");
                var iterationExperimentSheet = excel.Workbook.Worksheets.Add("Algorithm iterations experiment");
                var BExperimentSheet = excel.Workbook.Worksheets.Add("B constant experiment");
                var selectionExperimentSheet =
                    excel.Workbook.Worksheets.Add("Genetic selection experiment");
                var optimizationExperimentSheet =
                    excel.Workbook.Worksheets.Add("Genetic optimization experiment");

                WriteInputDataToSheet(inputDataSheet, inputData);
                WriteSizeExperimentToSheet(sizeExperimentSheet, sizeData, sizeExperimentResults);
                WriteIterationExperimentToSheet(iterationExperimentSheet, iterData, iterationExperimentResult);
                WriteBExperimentToSheet(BExperimentSheet, BData, BExperimentResult);
                WriteSelectionExperimentToSheet(selectionExperimentSheet, selectionIterData, selectionExperimentResult);
                WriteOptimizationExperimentToSheet(optimizationExperimentSheet, optimizationIterData,
                    optimizationExperimentResult);

                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var fileName = $"Experiments {DateTime.Now}.xlsx".Replace(":", ".");
                excel.SaveAs(new FileInfo($@"{location}\{fileName}"));
            }
        }

        private static void WriteInputDataToSheet(ExcelWorksheet sheet, InputData inputData)
        {
            sheet.Cells[1, 1].Value = "a";

            for (int i = 0; i < inputData.n; i++)
            {
                sheet.Cells[1, i + 2].Value = inputData.a[i];
            }

            sheet.Cells[2, 1].Value = "b";

            for (int i = 0; i < inputData.n; i++)
            {
                sheet.Cells[2, i + 2].Value = inputData.b[i];
            }

            sheet.Cells[3, 1].Value = "n";
            sheet.Cells[3, 2].Value = inputData.n;

            sheet.Cells[4, 1].Value = "B";
            sheet.Cells[4, 2].Value = inputData.B;
        }

        private static void WriteSizeExperimentToSheet(ExcelWorksheet sheet, List<int> sizeData,
            List<(Report greedy, Report bee, Report genetic)> results)
        {
            int index = 0;
            
            sheet.Cells[2, 1].Value = "Size";

            index = 2;
            foreach (var n in sizeData)
            {
                index++;
                sheet.Cells[index, 1].Value = n;
            }

            sheet.Cells[1, 2].Value = "Greedy";
            sheet.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; 
            sheet.Cells[1, 2, 1, 4].Merge = true;

            sheet.Cells[2, 2].Value = "Time";
            sheet.Cells[2, 3].Value = "Max value";
            sheet.Cells[2, 4].Value = "Min value";

            index = 2;
            foreach (var result in results)
            {
                index++;
                sheet.Cells[index, 2].Value = result.greedy.Time;
                sheet.Cells[index, 3].Value = result.greedy.ForMax.Result;
                sheet.Cells[index, 4].Value = result.greedy.ForMin.Result;
            }
            
            sheet.Cells[1, 5].Value = "Bee";
            sheet.Cells[1, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[1, 5, 1, 6].Merge = true;

            sheet.Cells[2, 5].Value = "Time";
            sheet.Cells[2, 6].Value = "Max value";

            index = 2;
            foreach (var result in results)
            {
                index++;
                sheet.Cells[index, 5].Value = result.bee.Time;
                sheet.Cells[index, 6].Value = result.bee.ForMax.Result;
            }

            sheet.Cells[1, 7].Value = "Genetic";
            sheet.Cells[1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[1, 7, 1, 9].Merge = true;

            sheet.Cells[2, 7].Value = "Time";
            sheet.Cells[2, 8].Value = "Max value";
            sheet.Cells[2, 9].Value = "Min value";

            index = 2;
            foreach (var result in results)
            {
                index++;
                sheet.Cells[index, 7].Value = result.genetic.Time;
                sheet.Cells[index, 8].Value = result.genetic.ForMax.Result;
                sheet.Cells[index, 9].Value = result.genetic.ForMin.Result;
            }
        
            SetChart(sheet, "Max value per IT size", (2, 10), $"A3:A{index}",
                ("Greedy", $"C3:C{index}"), ("Bee", $"F3:F{index}"), ("Genetic", $"H3:H{index}"));
            
            SetChart(sheet, "Min value per IT size", (18, 10), $"A3:A{index}",
                ("Greedy", $"D3:D{index}"), ("Genetic", $"I3:I{index}"));

            SetChart(sheet, "Time per IT size", (34, 10), $"A3:A{index}",
                ("Greedy", $"B3:B{index}"), ("Bee", $"E3:E{index}"), ("Genetic", $"G3:G{index}"));

        }

        private static void WriteIterationExperimentToSheet(ExcelWorksheet sheet, List<int> iterData,
            List<(Report greedy, Report bee, Report genetic)> results)
        {
            int index = 0;
            
            sheet.Cells[2, 1].Value = "Iterations";

            index = 2;
            foreach (var interations in iterData)
            {
                index++;
                sheet.Cells[index, 1].Value = interations;
            }
            
            sheet.Cells[1, 2].Value = "Bee";
            sheet.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[1, 2, 1, 3].Merge = true;
            
            sheet.Cells[2, 2].Value = "Time";
            sheet.Cells[2, 3].Value = "Max value";

            index = 2;
            foreach (var result in results)
            {
                index++;
                sheet.Cells[index, 2].Value = result.bee.Time;
                sheet.Cells[index, 3].Value = result.bee.ForMax.Result;
            }
            
            sheet.Cells[1, 4].Value = "Genetic";
            sheet.Cells[1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[1, 4, 1, 6].Merge = true;

            sheet.Cells[2, 4].Value = "Time";
            sheet.Cells[2, 5].Value = "Max value";
            sheet.Cells[2, 6].Value = "Min value";

            index = 2;
            foreach (var result in results)
            {
                index++;
                sheet.Cells[index, 4].Value = result.genetic.Time;
                sheet.Cells[index, 5].Value = result.genetic.ForMax.Result;
                sheet.Cells[index, 6].Value = result.genetic.ForMin.Result;
            }
        
            SetChart(sheet, "Max value per iterations", (2, 7), $"A3:A{index}",
                ("Bee", $"C3:C{index}"), ("Genetic", $"E3:E{index}"));

            SetChart(sheet, "Min value per iterations", (18, 7), $"A3:A{index}", ("Genetic", $"F3:F{index}"));

            SetChart(sheet, "Time per iterations", (34, 7), $"A3:A{index}",
                ("Bee", $"B3:B{index}"), ("Genetic", $"D3:D{index}"));
        }
        
        private static void WriteBExperimentToSheet(ExcelWorksheet sheet, List<int> BData,
            List<(Report greedy, Report bee, Report genetic)> results)
        {
            int index = 0;
            
            sheet.Cells[2, 1].Value = "B";

            index = 2;
            foreach (var B in BData)
            {
                index++;
                sheet.Cells[index, 1].Value = B;
            }

            sheet.Cells[1, 2].Value = "Greedy";
            sheet.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[1, 2, 1, 4].Merge = true;

            sheet.Cells[2, 2].Value = "Time";
            sheet.Cells[2, 3].Value = "Max value";
            sheet.Cells[2, 4].Value = "Min value";

            index = 2;
            foreach (var result in results)
            {
                index++;
                sheet.Cells[index, 2].Value = result.greedy.Time;
                sheet.Cells[index, 3].Value = result.greedy.ForMax.Result;
                sheet.Cells[index, 4].Value = result.greedy.ForMin.Result;
            }
            
            sheet.Cells[1, 5].Value = "Bee";
            sheet.Cells[1, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[1, 5, 1, 6].Merge = true;

            sheet.Cells[2, 5].Value = "Time";
            sheet.Cells[2, 6].Value = "Max value";

            index = 2;
            foreach (var result in results)
            {
                index++;
                sheet.Cells[index, 5].Value = result.bee.Time;
                sheet.Cells[index, 6].Value = result.bee.ForMax.Result;
            }

            sheet.Cells[1, 7].Value = "Genetic";
            sheet.Cells[1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[1, 7, 1, 9].Merge = true;

            sheet.Cells[2, 7].Value = "Time";
            sheet.Cells[2, 8].Value = "Max value";
            sheet.Cells[2, 9].Value = "Min value";

            index = 2;
            foreach (var result in results)
            {
                index++;
                sheet.Cells[index, 7].Value = result.genetic.Time;
                sheet.Cells[index, 8].Value = result.genetic.ForMax.Result;
                sheet.Cells[index, 9].Value = result.genetic.ForMin.Result;
            }
        
            SetChart(sheet, "Max value per B value", (2, 10), $"A3:A{index}",
                ("Greedy", $"C3:C{index}"), ("Bee", $"F3:F{index}"), ("Genetic", $"H3:H{index}"));
            
            SetChart(sheet, "Min value per B value", (18, 10), $"A3:A{index}",
                ("Greedy", $"D3:D{index}"), ("Genetic", $"I3:I{index}"));

            SetChart(sheet, "Time per B value", (34, 10), $"A3:A{index}",
                ("Greedy", $"B3:B{index}"), ("Bee", $"E3:E{index}"), ("Genetic", $"G3:G{index}"));
        }

        private static void WriteSelectionExperimentToSheet(ExcelWorksheet sheet, List<int> selectionIterData,
            List<(Report outbreeding, Report random, Report bestrandom, Report tournament)> results)
        {
            int index = 0;
            
            sheet.Cells[2, 1].Value = "Iterations";

            index = 2;
            foreach (var iterations in selectionIterData)
            {
                index++;
                sheet.Cells[index, 1].Value = iterations;
            }

            sheet.Cells[1, 2].Value = "Outbreeding";
            sheet.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[1, 2, 1, 4].Merge = true;

            sheet.Cells[2, 2].Value = "Time";
            sheet.Cells[2, 3].Value = "Max value";
            sheet.Cells[2, 4].Value = "Min value";

            index = 2;
            foreach (var result in results)
            {
                index++;
                sheet.Cells[index, 2].Value = result.outbreeding.Time;
                sheet.Cells[index, 3].Value = result.outbreeding.ForMax.Result;
                sheet.Cells[index, 4].Value = result.outbreeding.ForMin.Result;
            }
            
            sheet.Cells[1, 5].Value = "Random";
            sheet.Cells[1, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[1, 5, 1, 7].Merge = true;

            sheet.Cells[2, 5].Value = "Time";
            sheet.Cells[2, 6].Value = "Max value";
            sheet.Cells[2, 7].Value = "Min value";

            index = 2;
            foreach (var result in results)
            {
                index++;
                sheet.Cells[index, 5].Value = result.random.Time;
                sheet.Cells[index, 6].Value = result.random.ForMax.Result;
                sheet.Cells[index, 7].Value = result.random.ForMin.Result;
            }
            
            sheet.Cells[1, 8].Value = "Best and random";
            sheet.Cells[1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[1, 8, 1, 10].Merge = true;

            sheet.Cells[2, 8].Value = "Time";
            sheet.Cells[2, 9].Value = "Max value";
            sheet.Cells[2, 10].Value = "Min value";

            index = 2;
            foreach (var result in results)
            {
                index++;
                sheet.Cells[index, 8].Value = result.bestrandom.Time;
                sheet.Cells[index, 9].Value = result.bestrandom.ForMax.Result;
                sheet.Cells[index, 10].Value = result.bestrandom.ForMin.Result;
            }
            
            sheet.Cells[1, 11].Value = "Tournament";
            sheet.Cells[1, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[1, 11, 1, 13].Merge = true;

            sheet.Cells[2, 11].Value = "Time";
            sheet.Cells[2, 12].Value = "Max value";
            sheet.Cells[2, 13].Value = "Min value";

            index = 2;
            foreach (var result in results)
            {
                index++;
                sheet.Cells[index, 11].Value = result.tournament.Time;
                sheet.Cells[index, 12].Value = result.tournament.ForMax.Result;
                sheet.Cells[index, 13].Value = result.tournament.ForMin.Result;
            }

            SetChart(sheet, "Max value per iterations", (2, 14), $"A3:A{index}",
                ("Outbreeding", $"C3:C{index}"), ("Random", $"F3:F{index}"), ("Best and random", $"I3:I{index}"),
                ("Tournament", $"L3:L{index}"));
            
            SetChart(sheet, "Min value per iterations", (18, 14), $"A3:A{index}",
                ("Outbreeding", $"D3:D{index}"), ("Random", $"G3:G{index}"),  ("Best and random", $"J3:J{index}"),
                ("Tournament", $"M3:M{index}"));

            SetChart(sheet, "Time per iterations", (34, 14), $"A3:A{index}",
                ("Outbreeding", $"B3:B{index}"), ("Random", $"E3:E{index}"), ("Best and random", $"H3:H{index}"),
                ("Tournament", $"K3:K{index}"));
        }

        private static void WriteOptimizationExperimentToSheet(ExcelWorksheet sheet, List<int> iterData,
            List<(Report optimized, Report non_optimized)> results)
        {
            int index = 0;
            
            sheet.Cells[2, 1].Value = "Iterations";

            index = 2;
            foreach (var iterations in iterData)
            {
                index++;
                sheet.Cells[index, 1].Value = iterations;
            }

            sheet.Cells[1, 2].Value = "Optimized";
            sheet.Cells[1, 2, 1, 4].Merge = true;

            sheet.Cells[2, 2].Value = "Time";
            sheet.Cells[2, 3].Value = "Max value";
            sheet.Cells[2, 4].Value = "Min value";

            index = 2;
            foreach (var result in results)
            {
                index++;
                sheet.Cells[index, 2].Value = result.optimized.Time;
                sheet.Cells[index, 3].Value = result.optimized.ForMax.Result;
                sheet.Cells[index, 4].Value = result.optimized.ForMin.Result;
            }

            sheet.Cells[1, 5].Value = "Non-optimized";
            sheet.Cells[1, 5, 1, 7].Merge = true;

            sheet.Cells[2, 5].Value = "Time";
            sheet.Cells[2, 6].Value = "Max value";
            sheet.Cells[2, 7].Value = "Min value";

            index = 2;
            foreach (var result in results)
            {
                index++;
                sheet.Cells[index, 5].Value = result.non_optimized.Time;
                sheet.Cells[index, 6].Value = result.non_optimized.ForMax.Result;
                sheet.Cells[index, 7].Value = result.non_optimized.ForMin.Result;
            }

            SetChart(sheet, "Max value per iterations", (2, 8), $"A3:A{index}",
                ("Optimized", $"C3:C{index}"), ("Non-optimized", $"F3:F{index}"));
            
            SetChart(sheet, "Min value per iterations", (18, 8), $"A3:A{index}",
                ("Optimized", $"D3:D{index}"), ("Non-optimized", $"G3:G{index}"));

            SetChart(sheet, "Time per iterations", (34, 8), $"A3:A{index}",
                ("Optimized", $"B3:B{index}"), ("Non-optimized", $"E3:E{index}"));

        }
        private static void SetChart(ExcelWorksheet sheet, string name, (int Row, int Column) position, string xSerie,
            params (string name, string address)[] series)
        {
            var chart = sheet.Drawings.AddChart(name, eChartType.Line);
            chart.Title.Text = name;
            chart.SetSize(500, 300);
            chart.SetPosition(position.Row, 0, position.Column, 0);

            foreach (var serieData in series)
            {
                var serie = chart.Series.Add(serieData.address, xSerie);
                serie.Header = serieData.name;
            }
        }
    }
}