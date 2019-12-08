using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Advent7
{
    internal class Processor
    {
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

        public ConcurrentQueue<int> InputQueue { get; } = new ConcurrentQueue<int>();

        public ConcurrentQueue<int> OutputQueue { get; } = new ConcurrentQueue<int>();

        public event EventHandler<OutputProducedEventArgs> OutputProduced;
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(0, 1);

        public int[] WorkingSet { get; set; }

        public async Task Process()
        {
            var workingSet = WorkingSet;
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
                        position = await ExecuteInput(workingSet, position);
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
        }

        public void AddInput(int value)
        {
            InputQueue.Enqueue(value);
            _semaphoreSlim.Release();
        }

        private int ExecuteGoto(int[] workingSet, int position, int input, bool compareTo)
        {
            var x = GetParameter(workingSet, position + 1, GetParameterMode(input, 0));
            var y = GetParameter(workingSet, position + 2, GetParameterMode(input, 1));
            if ((x != 0) == compareTo) return y;
            return position + 3;
        }

        private int Execute2Params(int[] workingSet, int position, int input, Func<int, int, int> operation)
        {
            var x = GetParameter(workingSet, position + 1, GetParameterMode(input, 0));
            var y = GetParameter(workingSet, position + 2, GetParameterMode(input, 1));
            var result = operation(x, y);
            workingSet[workingSet[position + 3]] = result;
            return position + 4;
        }

        private async Task<int> ExecuteInput(int[] workingSet, int position)
        {
            int value;
            while (!InputQueue.TryDequeue(out value))
            {
                await _semaphoreSlim.WaitAsync(50);
            }
            var inputPosition = workingSet[position + 1];
            workingSet[inputPosition] = value;
            return position + 2;
        }

        private int ExecuteOutput(int[] workingSet, int position, int input)
        {
            var outputPosition = position + 1;
            var x = GetParameter(workingSet, outputPosition, GetParameterMode(input, 0));
            OutputQueue.Enqueue(x);
            OutputProduced?.Invoke(this, new OutputProducedEventArgs(x));
            return position + 2;
        }

        private ParameterMode GetParameterMode(int input, int parameterNumber)
        {
            return (ParameterMode)(input / (int)Math.Pow(10, parameterNumber + 2) % 10);
        }

        private int GetParameter(int[] workingSet, int position, ParameterMode mode)
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
