using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var input = await GetInput();

            /* For 1st star
            input[1] = 12;
            input[2] = 2;
            var output = Process(input);
            */

            // For 2nd star
            var output = Find(input, 19690720);

            Console.WriteLine(string.Join(",", output));
        }

        private static async Task<int[]> GetInput()
        {
            using (var reader = new StreamReader("./input.txt", true))
            {
                var content = await reader.ReadToEndAsync();
                return content.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.Parse(s)).ToArray();
            }
        }

        private static (int x, int y, int? result) Find(int[] input, int toFind)
        {
            var x = -1;
            var y = 0;
            int? result = null;
            do
            {
                x++;
                if (x == 100)
                {
                    x = 0;
                    y++;
                }
                if (y == 100)
                {
                    break;
                }
                var workingSet = new int[input.Length];
                input.CopyTo(workingSet, 0);
                try
                {
                    workingSet[1] = x;
                    workingSet[2] = y;
                    result = Process(workingSet)[0];
                }
                catch
                {
                }
            } while (result != toFind);
            return (x, y, result);
        }

        enum Instruction
        {
            Add = 1,
            Multiply = 2,
            Quit = 99
        }

        private static int[] Process(int[] workingSet)
        {
            Instruction currentInstruction;
            int position = 0;
            do
            {
                currentInstruction = (Instruction)workingSet[position];
                if (currentInstruction == Instruction.Quit) continue;

                var x = workingSet[workingSet[position + 1]];
                var y = workingSet[workingSet[position + 2]];
                int? result = null;
                switch (currentInstruction)
                {
                    case Instruction.Add:
                        result = x + y;
                        break;
                    case Instruction.Multiply:
                        result = x * y;
                        break;
                }
                workingSet[workingSet[position + 3]] = result ??
                    throw new InvalidOperationException("An invalid instruction was given");

                position += 4;
            } while (currentInstruction != Instruction.Quit);

            return workingSet;
        }
    }
}
