using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent15
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var input = await GetInput().ConfigureAwait(false);

            // For 1st star
            //await For1stStar(input).ConfigureAwait(false);

            // For 2nd star
            await For2ndStar(input).ConfigureAwait(false);
        }

        private static async Task For1stStar(long[] input)
        {
            var buffer = new Robot();
            buffer.ByTheRight = true;
            var processor = Initialize(input);
            processor.OutputProduced += buffer.ProcessorOnOutputProduced;
            processor.AddInput((int)Orientation.North);
            await processor.Process();
            buffer.PaintedTiles.Add((0, 0), Tile.Start);
            Render(buffer.PaintedTiles);
            Console.WriteLine($"Moves {buffer.Moves}");
        }

        private static async Task For2ndStar(long[] input)
        {
            var buffer = new Robot();
            buffer.ByTheRight = true;
            buffer.Explore = true;
            var processor = Initialize(input);
            processor.OutputProduced += buffer.ProcessorOnOutputProduced;
            processor.AddInput((int)Orientation.North);
            await processor.Process();
            buffer.PaintedTiles[(0, 0)] = Tile.Start;
            Render(buffer.PaintedTiles);

            var result = CountMinutes(buffer);
            Console.WriteLine($"Minutes to fill: {result}");
        }

        private static int CountMinutes(Robot buffer)
        {
            int result = 0;
            var tiles = buffer.PaintedTiles;
            while (tiles.Any(t => t.Value == Tile.Visited))
            {
                result++;
                foreach (var tile in tiles.Where(t => t.Value == Tile.Oxigen).ToArray())
                {
                    var tilePos = (tile.Key.x + 1, tile.Key.y);
                    if (tiles[tilePos] == Tile.Visited || tiles[tilePos] == Tile.Start)
                        tiles[tilePos] = Tile.Oxigen;
                    tilePos = (tile.Key.x - 1, tile.Key.y);
                    if (tiles[tilePos] == Tile.Visited || tiles[tilePos] == Tile.Start)
                        tiles[tilePos] = Tile.Oxigen;
                    tilePos = (tile.Key.x, tile.Key.y + 1);
                    if (tiles[tilePos] == Tile.Visited || tiles[tilePos] == Tile.Start)
                        tiles[tilePos] = Tile.Oxigen;
                    tilePos = (tile.Key.x, tile.Key.y - 1);
                    if (tiles[tilePos] == Tile.Visited || tiles[tilePos] == Tile.Start)
                        tiles[tilePos] = Tile.Oxigen;
                }
            }
            return result;
        }

        private static void Render(Dictionary<(int x, int y), Tile> output)
        {
            var xoffset = output.Keys.Min(k => k.x);
            var yoffset = output.Keys.Min(k => k.y);
            var width = output.Keys.Max(k => k.x) - xoffset + 1;
            var height = output.Keys.Max(k => k.y) - yoffset + 1;

            System.Console.WriteLine("Screen:");
            for (int y = 0; y < height; y++)
            {
                var rendered = new string(' ', width).ToCharArray();
                for (int x = 0; x < width; x++)
                {
                    var value = output.TryGetValue((x + xoffset, y + yoffset), out var t) ? t : Tile.Empty;
                    switch (value)
                    {
                        case Tile.Empty:
                            rendered[x] = ' ';
                            break;
                        case Tile.Visited:
                            rendered[x] = '.';
                            break;
                        case Tile.Wall:
                            rendered[x] = '\x2588';
                            break;
                        case Tile.Oxigen:
                            rendered[x] = 'O';
                            break;
                        case Tile.Start:
                            rendered[x] = 'X';
                            break;
                    }
                }
                System.Console.WriteLine(rendered);
            }
        }


        private static Processor Initialize(long[] input)
        {
            var processor = new Processor();
            var workingSet = new long[input.Length];
            input.CopyTo(workingSet, 0);
            processor.WorkingSet = workingSet;
            return processor;
        }

        private static async Task<long[]> GetInput()
        {
            using (var reader = new StreamReader("./input.txt", true))
            {
                var content = await reader.ReadToEndAsync().ConfigureAwait(false);
                return content.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => long.Parse(s, CultureInfo.InvariantCulture)).ToArray();
            }
        }
    }
}
