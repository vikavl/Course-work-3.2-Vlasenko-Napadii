using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace ConsoleApp1
{
    public enum SelectionType
    {
        Tournament,
        FullyRandom,
        PartlyRandom,
        OutBreeding
    }

    public enum LocalOptimization
    {
        Optimize,
        NotOptimize
    }
    
    public static class GeneticAlgorithm
    {
        class Gene
        {
            public Gene(int n, List<int> a, List<int> b)
            {
                Chromosomes = new List<int>();

                var random = new Random();
                for (int i = 0; i < n; i++)
                {
                    Chromosomes.Add(random.Next(a[i], b[i] + 1));
                }
            }
            
            private Gene(List<int> chromosomes)
            {
                Chromosomes = chromosomes;
            }

            public List<int> Chromosomes { get; }
            public int NumberOfChromosomes => Chromosomes.Count;
            public int Sum => Chromosomes.Sum();

            public int Fitness
            {
                get
                {
                    int result = 0;
                    for (int i = 0; i < NumberOfChromosomes - 1; i = i + 2)
                    {
                        result += Chromosomes[i] * Chromosomes[i + 1];
                    }

                    return result;
                }
            }

            public static Gene Recombinate(List<int> firstParent, List<int> secondParent, int n)
            {
                var genData = new List<int>();

                var random = new Random();
                for (int i = 0; i < n; i++)
                {
                    var alpha = random.Next(-1, 2) + random.NextDouble();
                    genData.Add((int) Math.Round(alpha * (secondParent[i] - firstParent[i])));
                }

                return new Gene(genData);
            }

            public void Mutate(List<int> a, List<int> b)
            {
                var random = new Random();
                for (int i = 0; i < NumberOfChromosomes; i++)
                {
                    var alpha = (b[i] - a[i]) / 2.0;
                    var beta = random.NextDouble();
                    
                    if (random.NextDouble() > 0.5)
                    {
                        Chromosomes[i] += (int) Math.Round(alpha * beta);
                    }
                    else
                    {
                        Chromosomes[i] -= (int) Math.Round(alpha * beta);
                    }
                }
            }

            public void Reanimate(List<int> a, List<int> b, int B)
            {
                for (int i = 0; i < NumberOfChromosomes; i++)
                {
                    if (Chromosomes[i] < a[i])
                    {
                        Chromosomes[i] = a[i];
                    }

                    else if(Chromosomes[i] > b[i])
                    {
                        Chromosomes[i] = b[i];
                    }

                    while (Sum > B)
                    {
                        int maxInx = 0;
                        var maxValue = Chromosomes[0];
                        for (int j = 1; j < NumberOfChromosomes; j++)
                        {
                            if (maxValue < Chromosomes[i])
                            {
                                maxValue = Chromosomes[i];
                                maxInx = i;
                            }
                        }

                        Chromosomes[maxInx]--;
                    }
                }
            }
            
            private bool isMax(List<int> a, List<int> b)
            {
                for (int i = 0; i < NumberOfChromosomes - 1; i += 2)
                {
                    if (Chromosomes[i] < 0 && Chromosomes[i + 1] < 0
                                           && (Chromosomes[i] > a[i] || Chromosomes[i + 1] > a[i + 1]))
                    {
                        return false;
                    }

                    if (Chromosomes[i] < b[i] && Chromosomes[i] >= 0 ||
                        Chromosomes[i + 1] < b[i + 1] && Chromosomes[i + 1] >= 0)
                    {
                        return false;
                    }
                }

                return true;
            }

            // Ціль оптимізації полягає в аналізі пар добутків. 
            // Пріоритетною задачею є максимізація межах допустимості мінімальних значень на кожній ітерації.
            // Винятком являються пари від’ємних добутків, та пари що мають обидва знаки. 
            // Від’ємні добутки необхідно навпаки мінімізувати межах допустимості і для порівняння зі
            // стандартним випадком беруться по модулю.
            // Для пар добутків, що мають обидва знаки кожне значення необхідно максимізувати в межах допустимості.
            // Для порівняння зі стандартним випадком від’ємні числа беруться без модулю.

            private enum MaxOptimizeType
            {
                Negative,
                Else,
                Default
            }
            public void OptimizeForMax(List<int> a, List<int> b, int B)
            {
                while (!isMax(a, b))
                {
                    if(Sum >= B) break;
                    
                    MaxOptimizeType type = MaxOptimizeType.Default;
                    int minInx = 0, minValue = Int32.MaxValue;
                    for (int i = 0; i < NumberOfChromosomes - 1; i += 2)
                    {
                        if (Chromosomes[i] < 0 && Chromosomes[i + 1] < 0)
                        {
                            if (minValue > Math.Abs(Chromosomes[i]) && Chromosomes[i] > a[i])
                            {
                                type = MaxOptimizeType.Negative;
                                minValue = Math.Abs(Chromosomes[i]);
                                minInx = i;
                            }

                            if(minValue > Math.Abs(Chromosomes[i + 1]) && Chromosomes[i + 1] > a[i + 1])
                            {
                                type = MaxOptimizeType.Negative;
                                minValue = Math.Abs(Chromosomes[i + 1]);
                                minInx = i + 1;
                            }
                        }

                        else 
                        {
                            if (minValue > Chromosomes[i] && Chromosomes[i] < b[i])
                            {
                                type = MaxOptimizeType.Else;
                                minValue = Chromosomes[i];
                                minInx = i;
                            }

                            if(minValue > Chromosomes[i + 1] && Chromosomes[i + 1] < b[i + 1])
                            {
                                type = MaxOptimizeType.Else;
                                minValue = Chromosomes[i + 1];
                                minInx = i + 1;
                            }
                        }
                    }

                    switch (type)
                    {
                        case MaxOptimizeType.Negative:
                            Chromosomes[minInx]--;
                            break;
                        case MaxOptimizeType.Else:
                            Chromosomes[minInx]++;
                            break;
                    }
                }
            }
            
            // Ціль оптимізації полягає в аналізі пар добутків. 
            // Пріоритетною задачею є мінімізація межах допустимості  максимальних значень на кожній ітерації.
            // Винятком являються пари від’ємних добутків, та пари що мають обидва знаки. 
            // Від’ємним добуткам необхідно надати значення максимально наближене до 0 і для порівняння зі
            // стандартним випадком беруться по модулю.
            // Для пар добутків, що мають обидва знаки, змінні з від’ємним знаком мінімізуються межах допустимості,
            // а змінні з додатнім максимізуються. Для порівняння зі стандартним випадком від’ємні числа беруться
            // з протилежним знаком.
            
            private enum MinOptimizeType
            {
                PositiveWithNegative,
                NegativeWithPositive,
                BothNegative,
                BothPositive,
                Default
            }
            
            private bool isMin(List<int> a, List<int> b)
            {
                for (int i = 0; i < NumberOfChromosomes - 1; i += 2)
                {
                    if (Chromosomes[i] < 0 && Chromosomes[i + 1] < 0 
                                           && (Chromosomes[i] < b[i] || Chromosomes[i + 1] < a[i + 1]))
                    {
                        return false;
                    }

                    if (Chromosomes[i] >= 0 && Chromosomes[i + 1] >= 0
                                           && (Chromosomes[i] > a[i] || Chromosomes[i + 1] > a[i + 1]))
                    {
                        return false;
                    }

                    if (Chromosomes[i] >= 0 && Chromosomes[i + 1] < 0 
                                           && (Chromosomes[i] < b[i] || Chromosomes[i + 1] > a[i + 1]))
                    {
                        return false;
                    }

                    if (Chromosomes[i] < 0 && Chromosomes[i + 1] >= 0
                                           && (Chromosomes[i] > a[i] || Chromosomes[i + 1] < b[i + 1]))
                    {
                        return false;
                    }
                }

                return true;
            }
            
            public void OptimizeForMin(List<int> a, List<int> b, int B)
            {
                while (!isMin(a, b))
                {
                    if(Sum >= B) break;
                    
                    MinOptimizeType type = MinOptimizeType.Default;
                    int maxInx = 0, maxValue = Int32.MinValue;
                    for (int i = 0; i < NumberOfChromosomes - 1; i += 2)
                    {
                        if (Chromosomes[i] < 0 && Chromosomes[i + 1] < 0)
                        {
                            if (maxValue < Math.Abs(Chromosomes[i]))
                            {
                                type = MinOptimizeType.BothNegative;
                                maxValue = Math.Abs(Chromosomes[i]);
                                maxInx = i;
                            }

                            if(maxValue < Math.Abs(Chromosomes[i + 1]))
                            {
                                type = MinOptimizeType.BothNegative;
                                maxValue = Math.Abs(Chromosomes[i + 1]);
                                maxInx = i + 1;
                            }
                        }

                        else if (Chromosomes[i] >= 0 && Chromosomes[i + 1] >= 0)
                        {
                            if (maxValue < Chromosomes[i] && Chromosomes[i] > a[i])
                            {
                                type = MinOptimizeType.BothPositive;
                                maxValue = Chromosomes[i];
                                maxInx = i;
                            }

                            if(maxValue < Chromosomes[i + 1] && Chromosomes[i + 1] > a[i + 1])
                            {
                                type = MinOptimizeType.BothPositive;
                                maxValue = Chromosomes[i + 1];
                                maxInx = i + 1;
                            }
                        }

                        else if (Chromosomes[i] >= 0 && Chromosomes[i + 1] < 0)
                        {
                            if (maxValue < Chromosomes[i] && Chromosomes[i] < b[i])
                            {
                                type = MinOptimizeType.PositiveWithNegative;
                                maxValue = Chromosomes[i];
                                maxInx = i;
                            }
                            
                            if(maxValue < Math.Abs(Chromosomes[i + 1]) && Chromosomes[i + 1] > a[i + 1])
                            {
                                type = MinOptimizeType.NegativeWithPositive;
                                maxValue = Math.Abs(Chromosomes[i + 1]);
                                maxInx = i + 1;
                            }
                        }
                        
                        else if (Chromosomes[i] < 0 && Chromosomes[i + 1] >= 0)
                        {
                            if (maxValue < Math.Abs(Chromosomes[i]) && Chromosomes[i] > a[i])
                            {
                                type = MinOptimizeType.NegativeWithPositive;
                                maxValue = Math.Abs(Chromosomes[i]);
                                maxInx = i;
                            }
                            
                            if(maxValue < Chromosomes[i + 1] && Chromosomes[i + 1] < b[i + 1])
                            {
                                type = MinOptimizeType.PositiveWithNegative;
                                maxValue = Chromosomes[i + 1];
                                maxInx = i + 1;
                            }
                        }
                    }

                    switch (type)
                    {
                        case MinOptimizeType.PositiveWithNegative:
                            Chromosomes[maxInx]++;
                            break;
                        case MinOptimizeType.NegativeWithPositive:
                            Chromosomes[maxInx]--;
                            break;
                        case MinOptimizeType.BothNegative:
                            Chromosomes[maxInx]++;
                            break;
                        case MinOptimizeType.BothPositive:
                            Chromosomes[maxInx]--;
                            break;
                    }
                }
            }
        }
        
        private static int NumberOfGenes { get; set; }
        private static List<Gene> Population { get; set; }
        
        private static Gene FirstParent { get; set; }
        private static Gene SecondParent { get; set; }
        
        private static Gene FirstChild { get; set; } 
        private static Gene SecondChild { get; set; }

        private static List<int> RecordsForMax { get; set; }
        private static List<int> RecordsForMin { get; set; }
        
        public static Report Calculate(
            int n, List<int> a, List<int> b, int B,
            SelectionType selectionType, LocalOptimization optimization, 
            int numberOfIterations = 10, int populationSize = 15)
        {
            if (numberOfIterations < 3) numberOfIterations = 3;
            NumberOfGenes = populationSize;
            RecordsForMax = new List<int>();
            int iterationsForMax = 0, recordForMax = int.MinValue;
            
            CreatePopulation(n, a, b, B);

            do
            {
                switch (selectionType)
                {
                    case SelectionType.Tournament: MakeTournamentSelectionForMax();
                        break;
                    case SelectionType.FullyRandom: MakeFullyRandomSelection();
                        break;
                    case SelectionType.PartlyRandom: MakeSelectionWithMaxAndRandom();
                        break;
                    case SelectionType.OutBreeding: MakeOutBreedingSelection();
                        break;
                }
                
                FirstChild = Gene.Recombinate(FirstParent.Chromosomes, SecondParent.Chromosomes, n);
                SecondChild = Gene.Recombinate(FirstParent.Chromosomes, SecondParent.Chromosomes, n);
                
                FirstChild.Mutate(a, b);
                SecondChild.Mutate(a, b);

                FirstChild.Reanimate(a, b, B);
                SecondChild.Reanimate(a, b, B);

                switch (optimization)
                {
                    case LocalOptimization.Optimize:
                        FirstChild.OptimizeForMax(a, b, B);
                        SecondChild.OptimizeForMax(a, b, B);
                        break;
                }
                
                UpdatePopulationForMax();

                if (recordForMax < Population[GetIndexOfMaxGen()].Fitness)
                    recordForMax = Population[GetIndexOfMaxGen()].Fitness;

                RecordsForMax.Add(recordForMax);
            } while (!isReadyToStop(RecordsForMax, ++iterationsForMax, numberOfIterations));
            
            CreatePopulation(n, a, b, B);

            RecordsForMin = new List<int>();

            int recordForMin = int.MaxValue, iterationsForMin = 0;
            
            do
            {
                switch (selectionType)
                {
                    case SelectionType.Tournament: MakeTournamentSelectionForMin();
                        break;
                    case SelectionType.FullyRandom: MakeFullyRandomSelection();
                        break;
                    case SelectionType.PartlyRandom: MakeSelectionWithMinAndRandom();
                        break;
                    case SelectionType.OutBreeding: MakeOutBreedingSelection();
                        break;
                }
                
                FirstChild = Gene.Recombinate(FirstParent.Chromosomes, SecondParent.Chromosomes, n);
                SecondChild = Gene.Recombinate(FirstParent.Chromosomes, SecondParent.Chromosomes, n);
                
                FirstChild.Mutate(a, b);
                SecondChild.Mutate(a, b);

                FirstChild.Reanimate(a, b, B);
                SecondChild.Reanimate(a, b, B);

                switch (optimization)
                {
                    case LocalOptimization.Optimize:
                        FirstChild.OptimizeForMin(a, b, B);
                        SecondChild.OptimizeForMin(a, b, B);
                        break;
                }
                
                UpdatePopulationForMin();

                if (recordForMin > Population[GetIndexOfMinGen()].Fitness)
                    recordForMin = Population[GetIndexOfMinGen()].Fitness;

                RecordsForMin.Add(recordForMin);
            } while (!isReadyToStop(RecordsForMin, ++iterationsForMin, numberOfIterations));

            return new Report(
                (recordForMax, iterationsForMax, RecordsForMax),
                (recordForMin, iterationsForMin, RecordsForMin));
        }

        private static bool isReadyToStop(List<int> recordHistory, int iterations, int minIterations)
        {
            if (iterations < minIterations)
                return false;
            
            var records = new List<int> {0, 0, 0};
            int index = recordHistory.Count;

            for (int i = 0; i < 3; i++)
            {
                records[i] = recordHistory[--index];
            }
            
            if (records[0] == records[1] && records[1] == records[2])
                return true;

            if (iterations >= minIterations * 10)
                return true;

            return false;
        }

        private static void MakeTournamentSelectionForMax()
        {
            var random = new Random();
            for (int i = 0; i < NumberOfGenes; i++)
            {
                var selection = random.Next(0, 2);
                if (selection == 0 && !(Population[i].Fitness <= FirstParent?.Fitness))
                    FirstParent = Population[i];
                else if (selection == 1 && !(Population[i].Fitness <= SecondParent?.Fitness))
                    SecondParent = Population[i];
            }
        }

        private static void MakeTournamentSelectionForMin()
        {
            var random = new Random();
            for (int i = 0; i < NumberOfGenes; i++)
            {
                var selection = random.Next(0, 2);
                if (selection == 0 && !(Population[i].Fitness >= FirstParent?.Fitness))
                    FirstParent = Population[i];
                else if (selection == 1 && !(Population[i].Fitness >= SecondParent?.Fitness))
                    SecondParent = Population[i];
            }
        }

        private static void MakeFullyRandomSelection()
        {
            var random = new Random();
            FirstParent = Population[random.Next(0, NumberOfGenes)];
            SecondParent = Population[random.Next(0, NumberOfGenes)];
        }

        private static void MakeSelectionWithMaxAndRandom()
        {
            var random = new Random();
            
            FirstParent = Population[0];
            for (int i = 1; i < NumberOfGenes; i++)
            {
                if (FirstParent.Fitness < Population[i].Fitness)
                    FirstParent = Population[i];
            }
            
            SecondParent = Population[random.Next(0, NumberOfGenes)];
        }
        
        private static void MakeSelectionWithMinAndRandom()
        {
            var random = new Random();
            
            FirstParent = Population[0];
            for (int i = 1; i < NumberOfGenes; i++)
            {
                if (FirstParent.Fitness > Population[i].Fitness)
                    FirstParent = Population[i];
            }
            
            SecondParent = Population[random.Next(0, NumberOfGenes)];
        }

        private static void MakeOutBreedingSelection()
        {
            FirstParent = Population[0];
            for (int i = 1; i < NumberOfGenes; i++)
            {
                if (FirstParent.Fitness < Population[i].Fitness)
                    FirstParent = Population[i];
            }
            
            SecondParent = Population[0];
            for (int i = 1; i < NumberOfGenes; i++)
            {
                if (SecondParent.Fitness > Population[i].Fitness)
                    SecondParent = Population[i];
            }
        }

        private static void CreatePopulation(int n, List<int> a, List<int> b, int B)
        {
            Gene gene = null;
            Population = new List<Gene>();

            for (int i = 0; i < NumberOfGenes; i++)
            {
                do
                {
                    gene = new Gene(n, a, b);
                    gene.Reanimate(a, b, B);
                } while (gene.Sum > B);

                Population.Add(gene);
            }
        }

        private static void UpdatePopulationForMax()
        {
            int minInx = GetIndexOfMinGen();
            if (Population[minInx].Fitness < FirstChild.Fitness)
                Population[minInx] = FirstChild;
            
            minInx = GetIndexOfMinGen();
            if (Population[minInx].Fitness < SecondChild.Fitness)
                Population[minInx] = SecondChild;
        }

        private static void UpdatePopulationForMin()
        {
            int maxInx = GetIndexOfMaxGen();
            if (Population[maxInx].Fitness > FirstChild.Fitness)
                Population[maxInx] = FirstChild;
            
            maxInx = GetIndexOfMaxGen();
            if (Population[maxInx].Fitness > SecondChild.Fitness)
                Population[maxInx] = SecondChild;
        }

        private static int GetIndexOfMinGen()
        {
            Gene minGene = Population[0];
            int index = 0;
            for (int i = 0; i < NumberOfGenes; i++)
            {
                if (minGene.Fitness > Population[i].Fitness)
                {
                    minGene = Population[i];
                    index = i;
                };
            }

            return index;
        }
        
        private static int GetIndexOfMaxGen()
        {
            Gene maxGene = Population[0];
            int index = 0;
            for (int i = 0; i < NumberOfGenes; i++)
            {
                if (maxGene.Fitness < Population[i].Fitness)
                {
                    maxGene = Population[i];
                    index = i;
                };
            }

            return index;
        }
    }
}