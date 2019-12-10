using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent10
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var input = await GetInput();
            Width = input.GetLength(0);
            Height = input.GetLength(1);
            var asteroids = GetAsteroids(input);

            // For 1st star
            var newBase = For1stStar(asteroids);

            //For 2nd star
            For2ndStar(asteroids, newBase);
        }

        private static (int x, int y) For1stStar((int x, int y)[] asteroids)
        {
            var observables = GetObservables(asteroids);
            var output = observables.OrderByDescending(o => o.o).First();
            Console.WriteLine($"x: {output.x}, y: {output.y}, observable: {output.o}");
            return (output.x, output.y);
        }

        private static void For2ndStar((int x, int y)[] asteroids, (int x, int y) asteroid)
        {
            var lines = GetSightLines(asteroids, asteroid);
            var i = 0;
            while (i < 200)
            {
                for (int l = 0; l < lines.Count; l++)
                {
                    if (lines.Count > 1)
                    {
                        var (x, y, _, _, _) = lines[l][0];
                        lines[l].RemoveAt(0);
                        i++;
                        if (i == 200)
                        {
                            Console.WriteLine($"200th asteroid destroyed at {x},{y}");
                            break;
                        }
                    }
                }
            }
        }

        static int Width;
        static int Height;

        private static (int x, int y)[] GetAsteroids(bool[,] map)
        {
            var result = new List<(int x, int y)>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (map[x, y]) result.Add((x, y));
                }
            }
            return result.ToArray();
        }

        private static (int x, int y, int o)[] GetObservables((int x, int y)[] asteroids)
        {
            var result = new (int x, int y, int o)[asteroids.Length];
            for (int i = 0; i < asteroids.Length; i++)
            {
                var asteroid = asteroids[i];
                var lines = GetSightLines(asteroids, asteroid);
                result[i] = (asteroid.x, asteroid.y, lines.Count);
            }
            return result;
        }

        private static List<List<(int x, int y, int dx, int dy, double d)>> GetSightLines((int x, int y)[] asteroids, (int x, int y) asteroid)
        {
            return asteroids
                .Where(a => a.x != asteroid.x || a.y != asteroid.y)
                .Select(a => GetVector(asteroid, a))
                .GroupBy(a => (a.dx, a.dy))
                .OrderBy(a => GetAngle(a.Key.dx, a.Key.dy))
                .Select(g => g.OrderBy(a => a.d).ToList()).ToList();
        }

        private static (int x, int y, int dx, int dy, double d) GetVector((int x, int y) a, (int x, int y) b)
        {
            var vx = a.x - b.x;
            var vy = a.y - b.y;
            var d = Math.Sqrt(Math.Pow(vx, 2) + Math.Pow(vy, 2));
            var gcd = GetGreatestCommonDivisor(Math.Abs(vx), Math.Abs(vy));
            if (gcd != 0)
            {
                vx /= gcd;
                vy /= gcd;
            }
            return (b.x, b.y, vx, vy, d);
        }

        private static int GetGreatestCommonDivisor(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }
            return a == 0 ? b : a;
        }

        private static double PI2 = Math.PI * 2;
        private static double GetAngle(int x, int y)
        {
            // Negative x to go clockwise
            // Negative angles counted back from 2PI
            // Results in angle in RAD increasing clockwise starting at top center = 0
            var angle = Math.Atan2(-x, y);
            if (angle < 0) angle = PI2 + angle;
            return angle;
        }

        static async Task<bool[,]> GetInput()
        {
            using (var reader = new StreamReader("input.txt", true))
            {
                var content = await reader.ReadToEndAsync();
                var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                var height = lines.Length;
                var width = lines[0].Length;
                var result = new bool[width, height];
                for (int y = 0; y < lines.Length; y++)
                {
                    lines[y].Select((c, x) => result[x, y] = c == '#').ToArray();
                }
                return result;
            }
        }
    }
}
