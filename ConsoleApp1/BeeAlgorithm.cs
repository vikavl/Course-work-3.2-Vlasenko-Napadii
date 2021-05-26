
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ConsoleApp1
{
    public class BeeAlgorithm
    {
        private static int B;
        private static int[] a;
        private static int[] b;
        
        //пчелиный алгоритм
        static int sum(int[] x)
        {
            int sum = 0;
            for (int i = 0; i < x.Length; i++)
            {
                sum += x[i];
            }
            return sum;
        }
        static bool admissibility(int[] x)
        {
            bool check = sum(x) <= B;
            return check;
        }
        
        static void generateArr(int[] arr)
        {
            Random rnd = new Random();
            for (int j = 0; j < arr.Length; j++)
            {
                arr[j]=rnd.Next(a[j], b[j]);
            }
        }
        
        static void generateDopust(int[] arr)
        {
            do 
            {
                generateArr(arr);
            } while (!admissibility(arr));
        }

        static int chooseMultType(int x1, int x2)
        {
            int temp = 0;
            if (x1 >= 0 && x2 >= 0) temp = 1; //++
            else if (x1 < 0 && x2 < 0) temp = 2; //--
            else temp = 3; //+-
            return temp;
        }

        static int multMin(int[] arr)
        {
            int mult_min = arr[0]*arr[1];
            for (int i = 0; i < arr.Length; i+=2)
            {
                if (arr[i] * arr[i + 1] < mult_min)
                {
                    mult_min = arr[i] * arr[i + 1];
                }
            }
            return mult_min;
        }

        static int biggerNumber(int x1, int x2)
        {
            int bigger = 0;
            if (x1 >= x2) bigger = x1;
            else bigger = x2;
            return bigger;
        }
        
        static void adaptArr(int[] arr)
        {
            do
            {
                for (int i = 0; i < arr.Length; i += 2)
                {
                    if (arr[i] * arr[i + 1] == multMin(arr))
                    {
                        if (arr[i] == biggerNumber(arr[i], arr[i + 1])) arr[i]--;
                        else arr[i + 1]--;
                    }
                }
            } while (!(sum(arr) <= B));
        }

        static int result(int[] arr)
        {
            int result = 0;
            for (int i = 0; i < arr.Length; i += 2)
            {
                result += arr[i] * arr[i + 1];
            }
            return result;
        }

        public static Report Calculate(int i, int n, int[] a, int[] b, int B)
        {
            BeeAlgorithm.a = a;
            BeeAlgorithm.b = b;
            BeeAlgorithm.B = B;
            
            int[] x = new int[n];
            int[] x_local = new int[n];
            int[] signs= new int[n];
            int resultBest = 0;

            var resultHistory = new List<int>();
            int iteration = 0;
            
            while (iteration < i)
            {
                generateDopust(x);
                for (int j = 0; j < x.Length; j+=2)
                {
                    signs[j] = signs[j+1] = chooseMultType(x[j], x[j + 1]);
                }
                
                generateDopust(x_local);
                for (int j = 0; j < signs.Length; j++)
                {
                    switch (signs[j])
                    {
                        case 1: //++
                        {
                            if (x[j] < x_local[j]) x[j] = x_local[j];
                            break;
                        }
                        case 2: //--
                        {
                            if (x[j] > x_local[j]) x[j] = x_local[j];
                            break;
                        }
                        case 3: //+-
                        {
                            if ((j+1) % 2 == 0) { if (x[j]*x[j-1] < x_local[j]*x[j-1] && j-1>=0) x[j] = x_local[j]; }
                            else { if (x[j]*x[j+1] < x_local[j]*x[j+1] && j+1<=n) x[j] = x_local[j]; }
                            break;
                        }
                    }
                }
                
                if (admissibility(x)==false)
                {
                    adaptArr(x);
                }
                
                if (resultBest < result(x)) resultBest = result(x);
                iteration++;

                resultHistory.Add(resultBest);
            }

            return new Report((resultBest, i, resultHistory), default);
        }
    }
}