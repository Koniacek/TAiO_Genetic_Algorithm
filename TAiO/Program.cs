using System;
using System.Diagnostics;

namespace TAiO
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = Stopwatch.StartNew();
            int[,] matrix1 =
            {
                {0,1,1,0},
                {1,0,0,1},
                {1,0,0,1},
                {0,1,1,0}
            };

            int[,] matrix2 =
            {
                {0,1,1,1},
                {1,0,1,1},
                {1,1,0,1},
                {1,1,1,0}
            };

            var g1 = new Graph(matrix1);
            var g2 = new Graph(matrix2);
            var generationSize = 100;
            var generationCount = 400;

            var algorithm = new GeneticAlgorithm(generationSize, generationCount);
            var solution = algorithm.FindMaximalCommonSubgraph(g1, g2);
            watch.Stop();
            Console.WriteLine(solution.ToString());
            Console.WriteLine($"{watch.ElapsedMilliseconds}ms, score={solution.Score}");
        }
    }
}
