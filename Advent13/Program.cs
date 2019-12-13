using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent13
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Correct for both 1st and 2nd star
            var input = await GetInput().ConfigureAwait(false);

            //await For1stStar(input).ConfigureAwait(false);

            // For 2nd star
            await For2ndStar(input).ConfigureAwait(false);
        }

        private static async Task For1stStar(long[] input)
        {
            var buffer = new Console();
            var processor = Initialize(input);
            processor.OutputProduced += buffer.ProcessorOnOutputProduced;
            await processor.Process();
            var output = buffer.PaintedTiles.Values.Count(t => t == Tile.Block);
            Render(buffer.PaintedTiles);
            System.Console.WriteLine($"Total block tiles: {output}");
        }

        private static async Task For2ndStar(long[] input)
        {
            var buffer = new Console();
            input[0] = 2;
            var processor = Initialize(input);
            processor.OutputProduced += buffer.ProcessorOnOutputProduced;
            await processor.Process();
            var output = buffer.Score;
            Render(buffer.PaintedTiles);
            System.Console.WriteLine($"Score: {output}");
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
                        case Tile.Wall:
                            rendered[x] = '\x2588';
                            break;
                        case Tile.Ball:
                            rendered[x] = '\x263A';
                            break;
                        case Tile.Paddle:
                            rendered[x] = '\x25AC';
                            break;
                        case Tile.Block:
                            rendered[x] = '\x2592';
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
