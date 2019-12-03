using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent3
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var input = await GetInput();
            var wires = FollowWires(input);
            var xings = FindXings(wires);
            /* For 1st star
            var distances = CalculateDistances(xings);
            */
           
            // For 2nd star
            var distances = CalculateXingDistances(xings);
            Console.WriteLine(string.Join(",", distances));
        }

        private static async Task<string[][]> GetInput()
        {
            using (var reader = new StreamReader("input.txt", true))
            {
                var content = await reader.ReadToEndAsync();
                var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                return lines.Select(l => l.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    .ToArray();
            }
        }

        private static List<List<(int x, int y, int d)>> FollowWires(string[][] input)
        {
            var result = new List<List<(int x, int y, int d)>>();

            for (int i = 0; i < input.Length; i++)
            {
                var wire = new List<(int x, int y, int d)>() { (0, 0, 0) };
                int x = 0;
                int y = 0;
                int d = 0;

                for (int j = 0; j < input[i].Length; j++)
                {
                    var step = input[i][j];
                    var direction = step[0];
                    var length = int.Parse(step.Substring(1));
                    for (int k = 0; k < length; k++)
                    {
                        d++;
                        switch (direction)
                        {
                            case 'R':
                                x++;
                                break;
                            case 'L':
                                x--;
                                break;
                            case 'U':
                                y++;
                                break;
                            case 'D':
                                y--;
                                break;
                        }
                        wire.Add((x, y, d));
                    }
                }
                result.Add(wire);
            }
            return result;
        }

        private static List<(int x, int y, int d)> FindXings(List<List<(int x, int y, int d)>> wires)
        {
            var result = wires[0].Join(wires[1], p1 => (p1.x, p1.y),
                p2 => (p2.x, p2.y), (p1, p2) => (p1.x, p1.y, p1.d + p2.d)).ToList();
            return result;
        }

        private static List<int> CalculateDistances(List<(int x, int y, int d)> xings)
        {
            var result = xings.Select(p => Math.Abs(p.x) + Math.Abs(p.y))
                .OrderBy(d => d).ToList();
            return result;
        }

        private static List<int> CalculateXingDistances(List<(int x, int y, int d)> xings)
        {
            var result = xings.Select(p => p.d)
                .OrderBy(d => d).ToList();
            return result;
        }

    }
}
