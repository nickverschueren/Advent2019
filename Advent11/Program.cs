using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Advent11
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
            var robot = new Robot();
            var processor = Initialize(input);
            processor.AddInput(0);
            processor.OutputProduced += robot.ProcessorOnOutputProduced;
            await processor.Process();
            var output = robot.PaintedTiles.Count;
            Console.WriteLine($"Total painted tiles: {output}");
        }

        private static async Task For2ndStar(long[] input)
        {
            var robot = new Robot();
            var processor = Initialize(input);
            processor.AddInput(1);
            processor.OutputProduced += robot.ProcessorOnOutputProduced;
            await processor.Process();
            var output = robot.PaintedTiles;
            Console.WriteLine($"Total painted tiles: {output.Count}");
            Render(output);
        }

        private static void Render(Dictionary<(int x, int y), bool> output)
        {
            var xoffset = output.Keys.Min(k => k.x);
            var yoffset = output.Keys.Min(k => k.y);
            var width = output.Keys.Max(k => k.x) - xoffset + 1;
            var height = output.Keys.Max(k => k.y) - yoffset + 1;

            Console.WriteLine("Message:");
            for (int y = height-1; y >= 0; y--)
            {
                var rendered = new string(' ', width).ToCharArray();
                for (int x = 0; x < width; x++)
                {
                    var value = output.TryGetValue((x + xoffset, y + yoffset), out var b) && b;
                    switch (value)
                    {
                        case false:
                            rendered[x] = ' ';
                            break;
                        case true:
                            rendered[x] = '\x2588';
                            break;
                    }
                }
                Console.WriteLine(rendered);
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
