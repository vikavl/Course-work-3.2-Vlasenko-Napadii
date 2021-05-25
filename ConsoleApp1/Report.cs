using System.Collections.Generic;

namespace ConsoleApp1
{
    public class Report
    {
        public (int Result, int Iterations, List<int> RecordHistory) ForMax { get; }
        
        public (int Result, int Iterations, List<int> RecordHistory) ForMin { get; }

        public Report(
            (int Result, int Iterations, List<int> RecordHistory) forMax,
            (int Result, int Iterations, List<int> RecordHistory) forMin)
        {
            ForMax = forMax;
            ForMin = forMin;
        }
    }
}