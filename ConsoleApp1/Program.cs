using System;

namespace ConsoleApp1
{
    class Program
    {
        public static int[] a = {-14, 12, 10, 0, -24, 3, 13, 7, -13, 2};

        public static int[] b = {10, 19, 22, 11, 1, 10, 13, 19, 1, 20};

        public static int B = 115;
        
        
        //пчелиный алгоритм
        public static int sum(int[] x)
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
        
        static void printArr(int[] arr)
        {
            for (int j = 0; j < arr.Length; j++)
            {
                Console.Write($"{arr[j]} ");
            }
            Console.WriteLine("");
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


            //printArr(arr);
            //Console.WriteLine("sum = "+sum(arr));
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

        static void beesAlgorithm(int i, int n)
        {
            int[] x = new int[n];
            int[] x_local = new int[n];
            int[] signs= new int[n];
            int resultBest = 0;

            while (i <= 1)
            {
                Console.WriteLine(i.ToString()+" iteration:");
                generateDopust(x);
                for (int j = 0; j < x.Length; j+=2)
                {
                    signs[j] = signs[j+1] = chooseMultType(x[j], x[j + 1]);
                }
                printArr(signs);
                Console.WriteLine("x_pochatkove: ");
                printArr(x);
                Console.WriteLine("result1 = " + result(x).ToString() + "; sum1 = " + sum(x).ToString());
                Console.WriteLine("x_localne: ");
                generateDopust(x_local);
                printArr(x_local);
                Console.WriteLine("local: result1 = " + result(x_local).ToString() + "; sum1 = " + sum(x_local).ToString());
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
                        default:
                        {
                            Console.WriteLine("Trouble");
                            break;
                        }
                    }
                }
                
                if (admissibility(x)==false)
                {
                    printArr(x);
                    Console.WriteLine("not adapted sum = " + sum(x).ToString());
                    adaptArr(x);
                    printArr(x);
                    Console.WriteLine("adapted sum = " + sum(x).ToString());
                    Console.WriteLine("result2 = " + result(x).ToString());
                }
                else
                {
                    Console.WriteLine("sum = " + sum(x).ToString());
                    printArr(x);
                    Console.WriteLine("result2 = " + result(x).ToString());
                }
                Console.WriteLine("------------");
                if (resultBest < result(x)) resultBest = result(x);
                i++;
            }
            Console.WriteLine("=================");
            Console.WriteLine("resultBest = " + resultBest.ToString());
        }
        
        
        
        //жадный алгоритм
        public static int sum1(int[] x, int n)
        {
            if (n < 0) n = 0;
            int sum = 0;
            for (int i = 0; i <= n; i++)
            {
                sum += x[i];
            }
            return sum;
        }

        static void Main(string[] args)
        {
            int i = 0, n = 10;
            //beesAlgorithm(i,n);
            int[] x = new int[n];
            int LowestResult = 0;
            int HighestResult = 0;
            while (i<n)
            {
                Console.WriteLine(i.ToString()+" iteration:");
                if (sum1(x, i - 1) + b[i] <= B) x[i] = b[i];
                else if (B - sum1(x, i - 1) >= a[i])
                {
                    x[i] = B - sum1(x, i - 1);
                }
                Console.WriteLine("sum = " + sum1(x, i).ToString());
                Console.WriteLine("x[" + i.ToString() + "] = " + x[i].ToString());
                printArr(x);
                HighestResult = result(x);
                Console.WriteLine("------------");
                i++;
            }
            Console.WriteLine("result (max) = " + HighestResult.ToString());
            Console.WriteLine("========================");
            i = 0;
            x = new int[n];
            while (i<n)
            {
                Console.WriteLine(i.ToString()+" iteration:");
                if (sum1(x, i - 1) + a[i] <= B) x[i] = a[i];
                Console.WriteLine("sum = " + sum(x).ToString());
                Console.WriteLine("x[" + i.ToString() + "] = " + x[i].ToString());
                printArr(x);
                LowestResult = result(x);
                Console.WriteLine("------------");
                i++;
            }
            Console.WriteLine("result (min) = " + LowestResult.ToString());
        }
    }
}