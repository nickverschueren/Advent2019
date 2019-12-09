using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent9
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Correct for both 1st and 2nd star
            var input = await GetInput().ConfigureAwait(false);

            /* For 1st star
            await For1stStar(input).ConfigureAwait(false);
            */

            // For 2nd star
            await For2ndStar(input).ConfigureAwait(false);
        }

        private static async Task For1stStar(long[] input)
        {
            var processor = Initialize(input);
            processor.InputQueue.Enqueue(1);
            await processor.Process();
            Console.WriteLine($"Output: {string.Join(',', processor.OutputQueue)}");
        }

        private static async Task For2ndStar(long[] input)
        {
            var processor = Initialize(input);
            processor.InputQueue.Enqueue(2);
            await processor.Process();
            Console.WriteLine($"Output: {string.Join(',', processor.OutputQueue)}");
        }

        private static Processor Initialize(params long[] input)
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
