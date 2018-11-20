using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace TAiO
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathToFile1, pathToFile2;
            int generationSize = 0, generationCount = 0;
            var breakWhenScoreDrops = false;
            if (args.Length < 4)
            {
                throw new ArgumentException("Too few arguments");
            }
            if (!string.IsNullOrEmpty(args[0])&&!string.IsNullOrEmpty(args[1]))
            {
                pathToFile1 = args[0];
                pathToFile2 = args[1];
            }
            else
            {
                throw new ArgumentException("Empty path arguments");
            }

            if (int.TryParse(args[2], out var value1)&&int.TryParse(args[3],out var value2))
            {
                generationSize = value1;
                generationCount = value2;
            }
            else
            {
                throw new ArgumentException("Could not parse generation size or generation count");
            }

            if (args.Length == 5 && bool.TryParse(args[4], out var value))
            {
                breakWhenScoreDrops = value;
            }

            var g1 = GraphLoader.LoadGraph(pathToFile1);
            var g2 = GraphLoader.LoadGraph(pathToFile2);

            var watch = Stopwatch.StartNew();

            var algorithm = new GeneticAlgorithm(generationSize, generationCount, breakWhenScoreDrops);
            var solution = algorithm.FindMaximalCommonSubgraph(g1, g2);

            Console.WriteLine(solution.ToString());
#if DEBUG

            Console.WriteLine($"{watch.ElapsedMilliseconds}ms, score={solution.Score}");
#endif
        }
    }
}
