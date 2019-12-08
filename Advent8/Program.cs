using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent8
{
    class Program
    {
        private static int width = 25;
        private static int height = 6;

        static async Task Main(string[] args)
        {
            var input = await GetInput();
            var checksum = GetChecksum(input);
            Console.WriteLine($"Checksum: {checksum}");

            Render(input);
        }

        private static int GetChecksum(int[][][] image)
        {
            var counts = image.Select(l => l.SelectMany(r => r)
                        .GroupBy(p => p).OrderBy(g => g.Key)
                        .Select(g => (g.Key, Count: g.Count()))
                        .ToArray()).ToArray();
            var leastZeros = counts.OrderBy(l => l.SingleOrDefault(g => g.Key == 0).Count).First();
            var ones = leastZeros.SingleOrDefault(g => g.Key == 1).Count;
            var twos = leastZeros.SingleOrDefault(g => g.Key == 2).Count;
            return ones * twos;
        }

        private static async Task<int[][][]> GetInput()
        {
            using (var reader = new StreamReader("input.txt", true))
            {
                var content = await reader.ReadToEndAsync();
                var layers = content.Length / width / height;
                var image = new int[layers][][];
                for (int z = 0; z < layers; z++)
                {
                    var layer = new int[height][];
                    for (int y = 0; y < height; y++)
                    {
                        var row = new int[width];
                        for (int x = 0; x < width; x++)
                        {
                            row[x] = int.Parse(content.Substring((z * height + y) * width + x, 1));
                        }
                        layer[y] = row;
                    }
                    image[z] = layer;
                }
                return image;
            }
        }

        private static void Render(int[][][] image)
        {
            Console.WriteLine("Message:");
            for (int y = 0; y < height; y++)
            {
                var line = new char[width];
                for (int x = 0; x < width; x++)
                {
                    for (int z = image.Length - 1; z >= 0; z--)
                    {
                        switch (image[z][y][x])
                        {
                            case 0:
                                line[x] = ' ';
                                break;
                            case 1:
                                line[x] = '\x2588';
                                break;
                        }
                    }
                }
                Console.WriteLine(line);
            }
        }

    }
}
