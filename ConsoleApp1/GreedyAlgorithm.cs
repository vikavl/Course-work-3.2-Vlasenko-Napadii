using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ConsoleApp1
{
    public static class GreedyAlgorithm
    {
        public static Report Calculate(int n, List<int> a, List<int> b, int B)
        {
            var minRecords = new List<int>();
            var maxRecords = new List<int>();
            
            var x = new List<int>();
            int sum = 0;
            
            for (int i = 0; i < n; i++)
            {
                int value = 0;
                if (sum + b[i] <= B)
                    value = b[i];
                
                else if (B - sum >= a[i])
                    value = B - sum;
                
                x.Add(value);
                sum += value;
                maxRecords.Add(GetFunctionResult(x, x.Count % 2 == 0 ? x.Count : x.Count - 1));
            }

            int maxValue = GetFunctionResult(x, n);

            x = new List<int>();
            sum = 0;

            for (int i = 0; i < n; i++)
            {
                int value = 0;
                if (sum + a[i] <= B)
                    value = a[i];
                
                x.Add(value);
                sum += value;
                minRecords.Add(GetFunctionResult(x, x.Count % 2 == 0 ? x.Count : x.Count - 1));
            }
            
            int minValue = GetFunctionResult(x, n);

            return new Report((maxValue, n, maxRecords), (minValue, n, minRecords));
        }

        private static int GetFunctionResult(List<int> x, int n)
        {
            int result = 0;
            for (int i = 0; i < n - 1; i = i + 2)
            {
                result += x[i] * x[i + 1];
            }

            return result;
        }
    }
}