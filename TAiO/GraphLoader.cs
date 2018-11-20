using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TAiO
{
    class GraphLoader
    {
        public static Graph LoadGraph(string pathToFile)
        {
            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException(pathToFile);
            }
            using (var reader = new StreamReader(pathToFile))
            {
                var lines = new List<string[]>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    lines.Add(values);
                }
                if (lines.Count > 0)
                {
                    var size = lines.Count;
                    var matrix = new int[size, size];
                    for (var i = 0; i < size; i++)
                    {
                        for (var j = 0; j < size; j++)
                        {
                            if (int.TryParse(lines[i][j], out var value))
                            {
                                if (value == 0 || value == 1)
                                {
                                    matrix[i, j] = value;
                                }
                                else
                                {
                                    throw new ArgumentOutOfRangeException("Possible values are 0 or 1");
                                }
                            }
                            else
                            {
                                throw new InvalidDataException($"Could not parse cell [{i},{j}]");
                            }

                        }
                    }
                    return new Graph(matrix);
                }
                else
                {
                    throw new Exception("Empty file");
                }
            }
        }
    }
}
