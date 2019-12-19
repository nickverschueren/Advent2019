using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent17
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
            var processor = Initialize(input);
            await processor.Process();
            Tile[][] output = GetOutput(processor.OutputQueue);
            Render(output);
            var crossings = GetCrossings(output);
            var total = crossings.Aggregate(0, (a, c) => a + (c.x * c.y));
            Console.WriteLine($"Total: {total}");
        }

        private static async Task For2ndStar(long[] input)
        {
            /*
             * Path split into segments:
                L12 R4 R4           A
                R12 R4 L12          B
                R12 R4 L12          B
                R12 R4 L6 L8 L8     C
                R12 R4 L6 L8 L8     C
                L12 R4 R4           A
                L12 R4 R4           A
                R12 R4 L12          B
                R12 R4 L12          B
                R12 R4 L6 L8 L8     C
             */

            var program = "A,B,B,C,C,A,A,B,B,C\n"
                        + "L,12,R,4,R,4\n"
                        + "R,12,R,4,L,12\n"
                        + "R,12,R,4,L,6,L,8,L,8\n"
                        + "n\n";
            input[0] = 2;
            var processor = Initialize(input);
            program.Select(c => Convert.ToByte(c)).ToList()
                .ForEach(b => processor.InputQueue.Enqueue(b));
            await processor.Process();
            var output = processor.OutputQueue.ToList();

            Render(GetOutput(processor.OutputQueue.Take(processor.OutputQueue.Count-1)));

            Console.WriteLine($"Output: {output.Last()}");
        }

        private static (int x, int y)[] GetCrossings(Tile[][] output)
        {
            var result = new List<(int x, int y)>();
            for (int y = 1; y < output.Length - 1; y++)
            {
                for (int x = 1; x < output[y].Length - 1; x++)
                {
                    if (output[y][x] == Tile.Path &&
                        output[y - 1][x] == Tile.Path &&
                        output[y + 1][x] == Tile.Path &&
                        output[y][x - 1] == Tile.Path &&
                        output[y][x + 1] == Tile.Path)
                        result.Add((x, y));
                }
            }
            return result.ToArray();
        }

        private static Tile[][] GetOutput(IEnumerable<long> video)
        {
            var output = new List<List<Tile>>();
            var line = new List<Tile>();
            var tiles = video.Select(c => (Tile)c).ToArray();
            for (int i = 0; i < tiles.Length; i++)
            {
                var tile = tiles[i];
                if (tile == Tile.EOL)
                {
                    if (line.Count > 0)
                        output.Add(line);
                    line = new List<Tile>();
                    continue;
                }
                line.Add(tile);
            }
            if (line.Count > 0)
                output.Add(line);
            return output.Select(l => l.ToArray()).ToArray();
        }

        private static void Render(Tile[][] output)
        {
            Console.WriteLine("Screen:");
            for (int y = 0; y < output.Length; y++)
            {
                Console.WriteLine(string.Join(null, output[y].Select(t => Convert.ToChar(t))));
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
