using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent7
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

        private static async Task For1stStar(int[] input)
        {
            List<int[]> output = new List<int[]>();
            var settings = new[] { 0, 1, 2, 3, 4 };
            foreach (var combination in EnumerateCombinations(settings))
            {
                var processors = Initialize(input, settings.Length);
                var result = new List<int>(combination);
                result.Insert(0, (await RunSequence(processors, combination, 0).ConfigureAwait(false))[0]);
                output.Add(result.ToArray());
            }
            var best = output.OrderByDescending(o => o[0]).First();

            Console.WriteLine($"Output : {string.Join(",", best)}");
        }

        private static async Task For2ndStar(int[] input)
        {
            List<int[]> output = new List<int[]>();
            var settings = new[] { 5, 6, 7, 8, 9 };
            foreach (var combination in EnumerateCombinations(settings))
            {
                var processors = Initialize(input, settings.Length);
                for (int i = 0; i < processors.Length; i++)
                {
                    var processor = processors[i];
                    var nextProcessor = processors[(i + 1) % 5];
                    processor.OutputProduced += (s, e) => nextProcessor.AddInput(e.Output);
                    processor.InputQueue.Enqueue(combination[i]);
                }
                processors[0].InputQueue.Enqueue(0);

                await Task.WhenAll(processors.Select(p => p.Process())).ConfigureAwait(false);

                var result = new List<int>(combination);
                result.Insert(0, processors.Last().OutputQueue.Last());
                output.Add(result.ToArray());

                Console.WriteLine($"Combination : {string.Join(",", result)}");
            }
            var best = output.OrderByDescending(o => o[0]).First();

            Console.WriteLine($"Output : {string.Join(",", best)}");
        }

        private static async Task<int[]> RunSequence(Processor[] processors, IList<int> combination, params int[] parameters)
        {
            for (int i = 0; i < combination.Count; i++)
            {
                var processor = processors[i];
                processor.InputQueue.Enqueue(combination[i]);
                parameters.ToList().ForEach(processor.InputQueue.Enqueue);
                processor.OutputQueue.Clear();
                await processor.Process().ConfigureAwait(false);
                parameters = processor.OutputQueue.ToArray();
            }
            return parameters;
        }

        private static Processor[] Initialize(int[] input, int count)
        {
            var result = new Processor[count];
            for (int i = 0; i < count; i++)
            {
                var processor = new Processor();
                var workingSet = new int[input.Length];
                input.CopyTo(workingSet, 0);
                processor.WorkingSet = workingSet;
                result[i] = processor;
            }
            return result;
        }

        private static async Task<int[]> GetInput()
        {
            using (var reader = new StreamReader("./input.txt", true))
            {
                var content = await reader.ReadToEndAsync().ConfigureAwait(false);
                return content.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.Parse(s, CultureInfo.InvariantCulture)).ToArray();
            }
        }

        private static IEnumerable<IList<int>> EnumerateCombinations(IReadOnlyList<int> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                var selection = values[i];
                if (values.Count == 1)
                {
                    yield return new List<int> { selection };
                    break;
                }
                var rest = values.Except(new[] { selection }).ToList();
                foreach (var combination in EnumerateCombinations(rest))
                {
                    combination.Insert(0, selection);
                    yield return combination;
                }
            }
        }
    }
}
