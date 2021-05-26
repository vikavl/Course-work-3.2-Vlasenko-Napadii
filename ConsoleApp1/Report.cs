using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace ConsoleApp1
{
    public class Report
    {
        public (int Result, int Iterations, List<int> RecordHistory) ForMax { get; }
        
        public (int Result, int Iterations, List<int> RecordHistory) ForMin { get; }

        public string Algorithm { get; set; }
        public double Time { get; set; }

        public Report(
            (int Result, int Iterations, List<int> RecordHistory) forMax,
            (int Result, int Iterations, List<int> RecordHistory) forMin)
        {
            ForMax = forMax;
            ForMin = forMin;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append($"{Algorithm} results. Time: {Time}s\n");

            if (ForMax != default)
            {
                builder.Append($"Maximum value = {ForMax.Result}. Iterations: {ForMax.Iterations}\n");
            }

            if (ForMin != default)
            {
                builder.Append($"Minimum value = {ForMin.Result}. Iterations: {ForMin.Iterations}\n");
            }

            builder.Append('\n');
            return builder.ToString();
        }
    }
}