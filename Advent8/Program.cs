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
        private static int size = width * height;
        private static int layers;

        static async Task Main(string[] args)
        {
            var input = GetInput();
            layers = input.Length / size;
            var checksum = GetChecksum(input);
            Console.WriteLine($"Checksum: {checksum}");

            Render(input);
        }

        private static int GetChecksum(byte[] image)
        {
            var counts = new int[layers][];
            for (int l = 0; l < layers; l++)
            {
                counts[l] = new int[3];
                for (int p = l * size; p < (l + 1) * size; p++)
                {
                    counts[l][image[p]] += 1;
                }
            }
            var leastZeros = counts.OrderBy(l => l[0]).First();
            var ones = leastZeros[1];
            var twos = leastZeros[2];
            return ones * twos;
        }

        private static void Render(byte[] image)
        {
            Console.WriteLine("Message:");
            for (int y = 0; y < height; y++)
            {
                var rendered = new string(' ', width).ToCharArray();
                for (int x = 0; x < width; x++)
                {
                    var offset = y * width + x;
                    for (int l = 0; l < layers; l++)
                    {
                        var value = image[offset];
                        switch (value)
                        {
                            case 0:
                                rendered[x] = ' ';
                                break;
                            case 1:
                                rendered[x] = '\x2588';
                                break;
                            case 2:
                                offset += size;
                                continue;
                        }
                        break;
                    }
                }
                Console.WriteLine(rendered);
            }
        }

        private static byte[] GetInput()
        {
            var fileInfo = new FileInfo("input.txt");
            byte[] buffer;
            using (var reader = new BinaryReader(fileInfo.OpenRead()))
            {
                buffer = reader.ReadBytes((int)fileInfo.Length);
            }
            var offset = Convert.ToByte('0');
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] -= offset;
            }
            return buffer;
        }

    }
}
