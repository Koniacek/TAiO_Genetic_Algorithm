using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace TAiO
{
    class GeneticAlgorithm
    {
        private readonly int _generationSize;
        private readonly int _generationCount;
        private List<Graph> _generation;

        public GeneticAlgorithm(int generationSize, int generationCount)
        {
            _generationSize = generationSize;
            _generationCount = generationCount;
        }

        public Graph FindMaximalCommonSubgraph(Graph g1, Graph g2)
        {
            _generation = new List<Graph>();
            var maxSize = g1.Size < g2.Size ? g1.Size : g2.Size;
            var generationScore = int.MinValue;
            CreateFirstGeneration(_generationSize,maxSize);
            for (var i = 0; i < _generationCount; i++)
            {
                AssignScores(g1,g2);
                _generation.Sort((graph1, graph2) => graph1.Score.CompareTo(graph2.Score));
                KillHalfOfTheGeneration();
                var babies=MakeBabies();
                _generation.AddRange(babies);
                ApplyMutation();
                var newGenerationScore = CalculateGenerationScore();
                //if (newGenerationScore < generationScore) break;
                generationScore = newGenerationScore;
                AssignScores(g1,g2);
                var standardDeviation = CalculateStandardDeviation();
                Console.WriteLine($"Generation #{i} score={newGenerationScore}, best score={_generation.Max(g=>g.Score)}, standard deviation={standardDeviation}");
            }

            var bestScore = _generation.Max(graph => graph.Score);
            return _generation.First(graph => graph.Score == bestScore);
        }

        private double CalculateStandardDeviation()
        {
            var result = 0.0;
            var average = _generation.Average(g => g.Score);
            foreach (var graph in _generation)
            {
                result += Math.Pow(graph.Score - average, 2);
            }

            return result;
        }

        private int CalculateGenerationScore()
        {
            return _generation.Sum(graph => graph.Score);
        }

        private void ApplyMutation()
        {
            foreach (var graph in _generation)
            {
                graph.Mutate();
            }
        }

        private List<Graph> MakeBabies()
        {
            var babies = new List<Graph>();
            var minScore = _generation.Min(graph => graph.Score);
            if (minScore <= 0)
            {
                _generation.ForEach(graph => graph.NormalizedScore = graph.Score - minScore + 1);
            }
            else
            {
                _generation.ForEach(graph => graph.NormalizedScore = graph.Score);
            }

            var maxScore = _generation.Max(graph => graph.NormalizedScore);
            while (babies.Count < _generationSize / 2)
            {
                var fatherIndex = SelectParentIndex(null);
                var motherIndex = SelectParentIndex(fatherIndex);
                var babyGraph = Graph.CreateChild(_generation[motherIndex], _generation[fatherIndex]);
                babies.Add(babyGraph);
            }

            return babies;
        }

        private int SelectParentIndex(int? blockedIndex)
        {
            var indices=new List<int>();
            for (var i = 0; i < _generation.Count; i++)
            {
                if(blockedIndex.HasValue&&blockedIndex.Value==i) continue;
                for (var j = 0; j < _generation[i].NormalizedScore; j++)
                {
                    indices.Add(i);
                }
            }

            var result = GoodRandom.Next(indices.Count);
            return indices[result];
        }

        private void KillHalfOfTheGeneration()
        {
            var indicesOfGraphsToRemove = new List<int>();
            var index = 0;
            while(indicesOfGraphsToRemove.Count<_generationSize/2)
            {
                if (GoodRandom.Next(_generationSize) > index&&!indicesOfGraphsToRemove.Contains(index))
                {
                    indicesOfGraphsToRemove.Add(index);
                }

                index = (index + 1) % _generationSize;
            }

            indicesOfGraphsToRemove.Sort((index1, index2) => -index1.CompareTo(index2));
            foreach (var i in indicesOfGraphsToRemove)
            {
                _generation.RemoveAt(i);
            }
        }

        private void CreateFirstGeneration(int generationSize, int maxSize)
        {
            for (var i = 0; i < generationSize; i++)
            {
                _generation.Add(Graph.CreateRandomGraph(maxSize));
            }
        }

        private void AssignScores(Graph g1, Graph g2)
        {
            foreach (var graph in _generation)
            {
                graph.Score = CalculateScore(g1, g2, graph);
            }
        }

        private static int CalculateScore(Graph g1, Graph g2, Graph target)
        {
            if (target.NumberOfUnconnectedSubgraphs > 1) return -(g1.Size + g2.Size);
            if (target.Size > g1.Size || target.Size > g2.Size) return -(g1.Size + g2.Size);
            if(target.EdgesCount>g1.EdgesCount||target.EdgesCount>g2.EdgesCount) return -(g1.Size + g2.Size);
            var n = 2 * target.EdgesCount;
            var v = CalculateV(g1, g2, target);
            var t1 = CalculateT(g1, target);
            var t2 = CalculateT(g2, target);
            return n - v - (t1 + t2 - 2);
        }

        private static int CalculateT(Graph g, Graph target)
        {
            return g.NumberOfUnconnectedSubgraphsInMatching(target);
        }

        private static int CalculateV(Graph g1, Graph g2, Graph target)
        {
            return 0;
        }
    }
}
