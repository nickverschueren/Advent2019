using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Correct for both 1st and 2nd star
            var input = await GetInput();
            var output = Process(input);

            Console.WriteLine(string.Join(",", output));
        }

        private static async Task<int[]> GetInput()
        {
            using (var reader = new StreamReader("./input.txt", true))
            {
                var content = await reader.ReadToEndAsync();
                return content.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.Parse(s, CultureInfo.InvariantCulture)).ToArray();
            }
        }

        enum Instruction
        {
            Add = 1,
            Multiply = 2,
            Input = 3,
            Output = 4,
            GotoIfTrue = 5,
            GotoIfFalse = 6,
            IsLessThan = 7,
            Equals = 8,
            Quit = 99
        }

        enum ParameterMode
        {
            Position = 0,
            Immediate = 1
        }

        private static int[] Process(int[] workingSet)
        {
            Instruction currentInstruction;
            int position = 0;
            do
            {
                var input = workingSet[position];

                currentInstruction = (Instruction)(input % 100);
                if (currentInstruction == Instruction.Quit) break;

                switch (currentInstruction)
                {
                    case Instruction.Add:
                        position = Execute2Params(workingSet, position, input, (x, y) => x + y);
                        break;
                    case Instruction.Multiply:
                        position = Execute2Params(workingSet, position, input, (x, y) => x * y);
                        break;
                    case Instruction.Input:
                        position = ExecuteInput(workingSet, position);
                        break;
                    case Instruction.Output:
                        position = ExecuteOutput(workingSet, position, input);
                        break;
                    case Instruction.GotoIfTrue:
                        position = ExecuteGoto(workingSet, position, input, true);
                        break;
                    case Instruction.GotoIfFalse:
                        position = ExecuteGoto(workingSet, position, input, false);
                        break;
                    case Instruction.IsLessThan:
                        position = Execute2Params(workingSet, position, input, (x, y) => x < y ? 1 : 0);
                        break;
                    case Instruction.Equals:
                        position = Execute2Params(workingSet, position, input, (x, y) => x == y ? 1 : 0);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown instruction {currentInstruction}"); ;
                }
            } while (true);

            return workingSet;
        }

        private static int ExecuteGoto(int[] workingSet, int position, int input, bool compareTo)
        {
            var x = GetParameter(workingSet, position + 1, GetParameterMode(input, 0));
            var y = GetParameter(workingSet, position + 2, GetParameterMode(input, 1));
            if ((x != 0) == compareTo) return y;
            return position + 3;
        }

        private static int Execute2Params(int[] workingSet, int position, int input, Func<int, int, int> operation)
        {
            var x = GetParameter(workingSet, position + 1, GetParameterMode(input, 0));
            var y = GetParameter(workingSet, position + 2, GetParameterMode(input, 1));
            var result = operation(x, y);
            workingSet[workingSet[position + 3]] = result;
            return position + 4;
        }

        private static int ExecuteInput(int[] workingSet, int position)
        {
            var inputPosition = workingSet[position + 1];
            int value;
            do
            {
                Console.WriteLine($"Input for position {inputPosition}:");
            } while (!int.TryParse(Console.ReadLine(), out value));

            workingSet[inputPosition] = value;
            return position + 2;
        }

        private static int ExecuteOutput(int[] workingSet, int position, int input)
        {
            var outputPosition = position + 1;
            var x = GetParameter(workingSet, outputPosition, GetParameterMode(input, 0));
            Console.WriteLine($"Output from position {outputPosition}: {x}");
            return position + 2;
        }

        private static ParameterMode GetParameterMode(int input, int parameterNumber)
        {
            return (ParameterMode)(input / (int)Math.Pow(10, parameterNumber + 2) % 10);
        }

        private static int GetParameter(int[] workingSet, int position, ParameterMode mode)
        {
            switch (mode)
            {
                case ParameterMode.Immediate:
                    return workingSet[position];
                case ParameterMode.Position:
                    return workingSet[workingSet[position]];
                default:
                    throw new ArgumentException($"Invalid mode {mode}");
            }
        }
    }
}
