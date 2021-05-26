using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace ConsoleApp1
{
    public class InputData
    {
        public List<int> a { get; set; }
        public List<int> b { get; set; }
    
        public int n { get; set; }
        public int B { get; set; }

        public InputData()
        {
        }

        public InputData(int n, List<int> a, List<int> b, int B)
        {
            this.n = n;
            this.a = a;
            this.b = b;
            this.B = B;
        }

        public string FileName => "InputData.json";
        public void GenerateInputData(int n, int aMin = -50, int aMax = 51, int bMax = 100)
        {
            var random = new Random();

            this.n = n;
            
            a = new List<int>();
            b = new List<int>();
            for (int i = 0; i < n; i++)
            {
                a.Add(random.Next(aMin, aMax));
                b.Add(random.Next(a[i] > 0 ? a[i] : 1, bMax));
            }

            var sum = a.Sum();
            var minValue = sum < 0 ? n : sum;
            var maxValue = b.Sum();
            B = random.Next(minValue, maxValue);
        }
        
        public void EnterInputData(int n)
        {
            this.n = n;

            a = new List<int>();
            b = new List<int>();
            
            Console.WriteLine("Enter values of a vector:");
            for (int i = 0; i < n; i++)
            {
                a.Add(Program.GetValidInput($"{i + 1}"));
            }

            Console.WriteLine();
            Console.WriteLine("Enter values of b vector:");
            for (int i = 0; i < n; i++)
            {
                b.Add(Program.GetValidInput(
                    $"{i + 1}",
                    value => value >= a[i],
                    "Value must be higher or equal then from a vector."));
            }

            Console.WriteLine();
            B = Program.GetValidInput(
                "Enter value of B constant",
                value => value > a.Sum() && value < b.Sum(),
                "Value must be lower then sum of a vector elements " +
                "and higher then sum of b vector elements.");
        }

        public void SaveInputData()
        {
            string data = JsonSerializer.Serialize(this);
            File.WriteAllText(FileName, data);
        }

        public void ReadInputData()
        {
            InputData instance = null;
            
            if (File.Exists(FileName))
            {
                string data = File.ReadAllText(FileName);
                instance = JsonSerializer.Deserialize<InputData>(data);
            }

            n = instance?.n ?? 0;
            a = instance?.a;
            b = instance?.b;
            B = instance?.B ?? 0;
        }

        public string ViewInputData()
        {
            var builder = new StringBuilder();

            builder.Append("a = [");
            
            foreach (var value in a)
            {
                builder.Append($"{value}, ".PadLeft(5));
            }

            builder.Remove(builder.Length - 2, 2);
            builder.Append("];\n");
            
            builder.Append("b = [");
            
            foreach (var value in b)
            {
                builder.Append($"{value}, ".PadLeft(5));
            }

            builder.Remove(builder.Length - 2, 2);
            builder.Append("];\n");

            builder.Append($"n = {n}; B = {B};\n");

            return builder.ToString();
        }
    }
}