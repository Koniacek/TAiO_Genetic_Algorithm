using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace TAiO
{
    class Graph
    {
        private int[,] _graph;

        public Graph(int[,] graph)
        {
            _graph = graph;
        }

        #region PropertiesWithoutLogic
        public int Score { get; set; }
        public int NormalizedScore { get; set; }

        #endregion

        #region PropertiesWithLogic

        public int Size => _graph.GetLength(0);
        public int EdgesCount
        {
            get
            {
                var count = 0;
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < i; j++)
                    {
                        if (_graph[i, j] == 1) count++;
                    }
                }
                return count;
            }
        }
        public int this[int i, int j] => _graph[i, j];
        public int NumberOfUnconnectedSubgraphs
        {
            get
            {
                var edges = new List<(int from, int to)>();
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        if (_graph[i, j] == 1 && !edges.Contains((j, i)))
                        {
                            edges.Add((i, j));
                        }
                    }
                }


                if (edges.Count == 0) return 0;
                var visited = new bool[Size];
                var queue = new Queue<int>();
                var subgraphsCount = 0;
                while (visited.Contains(false))
                {
                    for (var i = 0; i < Size; i++)
                    {
                        if (visited[i] == false)
                        {
                            queue.Enqueue(i);
                            break;
                        }
                    }
                    subgraphsCount++;
                    while (queue.Count > 0)
                    {
                        var p = queue.Dequeue();
                        if (!visited[p])
                        {
                            visited[p] = true;
                            foreach (var edge in edges.FindAll(e => e.from == p || e.to == p))
                            {
                                if (edge.from == p && !visited[edge.to])
                                {
                                    queue.Enqueue(edge.to);
                                }
                                else if (!visited[edge.from])
                                {
                                    queue.Enqueue(edge.from);
                                }
                            }
                        }
                    }


                }

                return subgraphsCount;
            }
        }

        #endregion

        #region PublicStaticMethods

        public static Graph CreateRandomGraph(int maxSize)
        {
            var size = GoodRandom.Next(maxSize) + 1;
            var matrix = new int[size, size];
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < i; j++)
                {
                    if (GoodRandom.Bool())
                    {
                        matrix[i, j] = 1;
                        matrix[j, i] = 1;
                    }
                }
            }

            return new Graph(matrix);
        }
        public static Graph CreateChild(Graph mother, Graph father)
        {
            var childSize = (mother.Size + father.Size) / 2;
            if ((mother.Size + father.Size) % 2 == 1 && GoodRandom.Bool()) childSize++;
            var child = new int[childSize, childSize];
            var decision = false;
            for (var i = 0; i < childSize; i++)
            {
                for (var j = 0; j < i; j++)
                {
                    var edgeExistance = mother.GetValueRound(i, j) + father.GetValueRound(i, j);
                    switch (edgeExistance)
                    {
                        case 0:
                            decision = GoodRandom.Next(100) < 1;
                            if (decision)
                            {
                                child[i, j] = 1;
                                child[j, i] = 1;
                            }
                            break;
                        case 1:
                            decision = GoodRandom.Next(100) < 49;
                            if (decision)
                            {
                                child[i, j] = 1;
                                child[j, i] = 1;
                            }
                            break;
                        case 2:
                            decision = GoodRandom.Next(100) < 98;
                            if (decision)
                            {
                                child[i, j] = 1;
                                child[j, i] = 1;
                            }
                            break;
                        default:
                            throw new ArgumentException("Parent graphs are invalid, values other than 0 or 1 found");
                    }
                }
            }
            return new Graph(child);
        }

        #endregion

        #region PublicMethods

        public void Mutate()
        {
            var vertexProbability = GoodRandom.Next(100);
            var edgeProbability = GoodRandom.Next(100);
            var signDecision = GoodRandom.Bool();
            var sign = signDecision ? -1 : 1;
            var vertexChange = GetChange(vertexProbability);
            var edgeChange = GetChange(edgeProbability);
            vertexChange *= sign;
            edgeChange *= sign;
            if (vertexChange != 0)
            {
                var newSize = Size + vertexChange;
                if (newSize > 0)
                {
                    if (newSize > Size)
                    {
                        AddRandomVertex(vertexChange);
                    }
                    else
                    {
                        RemoveRandomVertex(-vertexChange);
                    }

                }
            }

            if (edgeChange != 0)
            {
                if (edgeChange > 0)
                {
                    AddRandomEdges(edgeChange);
                }
                else
                {
                    RemoveRandomEdges(-edgeChange);
                }
            }
        }
        public int NumberOfUnconnectedSubgraphsInMatching(Graph match)
        {
            if (match.Size > Size) return int.MaxValue;
            if (!(_graph.Clone() is int[,] matrixWithoutMatch))
            {
                throw new InvalidCastException();
            }
            for (var i = 0; i < match.Size; i++)
            {
                for (var j = 0; j < match.Size; j++)
                {
                    if (match[i, j] == 1) matrixWithoutMatch[i, j] = 0;
                }
            }

            var edges = new List<(int from, int to)>();
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    if (matrixWithoutMatch[i, j] == 1 && !edges.Contains((j, i)))
                    {
                        edges.Add((i, j));
                    }
                }
            }


            if (edges.Count == 0) return 0;
            var visited = new bool[Size];
            var queue = new Queue<int>();
            var subgraphsCount = 0;
            while (visited.Contains(false))
            {
                for (var i = 0; i < Size; i++)
                {
                    if (visited[i] == false)
                    {
                        queue.Enqueue(i);
                        break;
                    }
                }
                subgraphsCount++;
                while (queue.Count > 0)
                {
                    var p = queue.Dequeue();
                    if (!visited[p])
                    {
                        visited[p] = true;
                        foreach (var edge in edges.FindAll(e => e.from == p || e.to == p))
                        {
                            if (edge.from == p && !visited[edge.to])
                            {
                                queue.Enqueue(edge.to);
                            }
                            else if (!visited[edge.from])
                            {
                                queue.Enqueue(edge.from);
                            }
                        }
                    }
                }


            }

            return subgraphsCount;
        }
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    stringBuilder.Append($"{_graph[i, j]} ");
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
        public Graph Clone()
        {
            return new Graph(_graph);
        }

        #endregion

        #region HelperMethods_AddingAndRemovingEdgesAndVertices

        private void AddRandomEdges(int numberOfEdgesToAdd)
        {
            var possibleEdges = GetPossibleEdges();
            if (possibleEdges.Count > 0)
            {
                for (var i = 0; i < numberOfEdgesToAdd; i++)
                {
                    var edge = possibleEdges[GoodRandom.Next(possibleEdges.Count)];
                    _graph[edge.from, edge.to] = 1;
                    _graph[edge.to, edge.from] = 1;
                    possibleEdges.Remove(edge);
                    if (possibleEdges.Count <= 0) break;
                }
            }
        }
        private void RemoveRandomEdges(int numberOfEdgesToRemove)
        {
            var edges = GetEdges();
            if (edges.Count > 0)
            {
                for (var i = 0; i < numberOfEdgesToRemove; i++)
                {
                    var edge = edges[GoodRandom.Next(edges.Count)];
                    _graph[edge.from, edge.to] = 0;
                    _graph[edge.to, edge.from] = 0;
                    edges.Remove(edge);
                    if (edges.Count <= 0) break;
                }
            }
        }
        private void AddRandomVertex(int numberOfVerticesToAdd)
        {
            var newSize = Size + numberOfVerticesToAdd;
            var newMatrix = new int[newSize, newSize];
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    newMatrix[i, j] = _graph[i, j];
                }
            }

            _graph = newMatrix;
        }
        private void RemoveRandomVertex(int numberOfVerticesToDelete)
        {
            int[,] newMatrix = null;
            var sortedVertices = SortVerticesBasedOnDegree();
            var verticesToDelete = sortedVertices
                .Take(numberOfVerticesToDelete).ToList();
            verticesToDelete.Sort((v1, v2) => -v1.CompareTo(v2));
            foreach (var vertex in verticesToDelete)
            {
                newMatrix = DeleteVertex(_graph, vertex);
            }
            _graph = newMatrix;
        }
        private static int[,] DeleteVertex(int[,] matrix, int index)
        {
            var oldSize = matrix.GetLength(0);
            var newSize = oldSize - 1;
            var newMatrix = new int[newSize, newSize];
            int iPrim = 0, jPrim = 0;
            for (var i = 0; i < oldSize; i++)
            {
                if (i == index) continue;
                for (var j = 0; j < oldSize; j++)
                {
                    if (j == index) continue;
                    newMatrix[iPrim, jPrim] = matrix[i, j];
                    jPrim++;
                }

                jPrim = 0;
                iPrim++;
            }
            return newMatrix;
        }
        private int[] SortVerticesBasedOnDegree()
        {
            var vertices = new List<(int vertex, int degree)>();
            for (var i = 0; i < Size; i++)
            {
                vertices.Add((i, GetVertexDegree(i)));
            }

            vertices.Sort((tuple1, tuple2) => tuple1.degree.CompareTo(tuple2.degree));
            return vertices
                .ConvertAll(tuple => tuple.vertex)
                .ToArray();
        }
        private List<(int from, int to)> GetEdges()
        {
            var edges = new List<(int, int)>();
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    if (_graph[i, j] == 1 && !edges.Contains((j, i)))
                    {
                        edges.Add((i, j));
                    }
                }
            }

            return edges;
        }
        private List<(int from, int to)> GetPossibleEdges()
        {
            var emptyPairs = new List<(int, int)>();
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    if (_graph[i, j] == 0 && i != j && !emptyPairs.Contains((j, i)))
                    {
                        emptyPairs.Add((i, j));
                    }
                }
            }

            return emptyPairs;
        }
        private int GetVertexDegree(int index)
        {
            var degree = 0;
            for (var i = 0; i < Size; i++)
            {
                if (_graph[index, i] == 1) degree++;
            }
            return degree;
        }
        private int GetValueRound(int i, int j)
        {
            return _graph[i % Size, j % Size];
        }
        private static int GetChange(int probability)
        {
            //gaussian probability should go here
            int change;
            switch (probability)
            {
                case 95:
                case 96:
                case 97:
                    change = 1;
                    break;
                case 98:
                    change = 2;
                    break;
                case 99:
                    change = 3;
                    break;
                default:
                    change = 0;
                    break;
            }

            return change;
        }

        #endregion
        
    }
}
