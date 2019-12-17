using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent16
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var input = await GetInput();

            For1stStar(input);
            For2ndStar(input);
        }

        private static void For1stStar(int[] input)
        {
            var workingSet = input;
            for (int i = 0; i < 100; i++)
            {
                workingSet = Process(workingSet);
            }
            Console.WriteLine($"Result: {string.Join(null, workingSet.Take(8))}");
        }

        private static void For2ndStar(int[] input)
        {
            /* Expaination:
             * - The 2nd half of the working set (10000 times the input) will always result 
             *   in the sum of the numbers following the current number (because the pattern 
             *   is expanded by more than half the array size)
             * - Numbers before the offset also do not infulence the eventual outcome for 
             *   this reason
             * - So the working set can be croppped to only the numbers following the offset
             * - By calculating the sum of number starting at the last an itteration can be 
             *   performed very quickly
             * - After 100 iterations, the first 8 digits are what we need.
             */

            var offset = int.Parse(string.Join(null, input.Take(7)));
            var length = (input.Length * 10000) - offset;
            var part1 = input.Skip(offset % input.Length).ToArray();
            var rest = length / input.Length;
            var workingSet = part1.Concat(Enumerable.Range(0, rest).SelectMany(i => input)).ToArray();

            for (int i = 0; i < 100; i++)
            {
                var k = 0;
                for (int j = workingSet.Length - 1; j >= 0; j--)
                {
                    workingSet[j] = k = (k + workingSet[j]) % 10;
                }
            }

            var result = workingSet.Take(8).ToArray();
            Console.WriteLine($"Result: {string.Join(null, result)}");
        }

        private static int[] Process(int[] workingSet)
        {
            int[] pattern = new int[] { 0, 1, 0, -1 };
            var result = new int[workingSet.Length];
            Parallel.ForEach(Enumerable.Range(0, workingSet.Length), i =>
            {
                var value = 0L;
                int x = 0;
                for (int j = 0; j < workingSet.Length; j++)
                {
                    if ((j + 1) % (i + 1) == 0) x = (x + 1) % pattern.Length;
                    value += workingSet[j] * pattern[x];
                }
                result[i] = (int)Math.Abs(value % 10);
            });
            return result;
        }

        private static async Task<int[]> GetInput()
        {
            using (var reader = new StreamReader("input.txt", true))
            {
                var content = await reader.ReadToEndAsync();
                var offset = Convert.ToByte('0');
                var result = content
                    .Select(c => Convert.ToByte(c) - offset)
                    .Where(b => b >= 0 && b <= 9)
                    .ToArray();
                return result;
            }
        }
    }
}
