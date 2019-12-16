using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Advent15
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
            SetBaseAddress = 9,
            Quit = 99
        }

        enum ParameterMode
        {
            Position = 0,
            Immediate = 1,
            Relative = 2
        }

        public ConcurrentQueue<long> InputQueue { get; } = new ConcurrentQueue<long>();

        public ConcurrentQueue<long> OutputQueue { get; } = new ConcurrentQueue<long>();

        public event EventHandler<OutputProducedEventArgs> OutputProduced;
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(0, 1);

        public long[] WorkingSet { get => _workingSet; set => _workingSet = value; }

        private int _baseAddress;
        private long[] _workingSet;

        private bool _quit = false;
        public void Quit() => _quit = true;

        public async Task Process()
        {
            Instruction currentInstruction;
            int position = 0;
            do
            {
                var input = _workingSet[position];

                currentInstruction = (Instruction)(input % 100);
                if (currentInstruction == Instruction.Quit) break;

                switch (currentInstruction)
                {
                    case Instruction.Add:
                        position = Execute2Params(position, input, (x, y) => x + y);
                        break;
                    case Instruction.Multiply:
                        position = Execute2Params(position, input, (x, y) => x * y);
                        break;
                    case Instruction.Input:
                        position = await ExecuteInput(position, input).ConfigureAwait(false);
                        break;
                    case Instruction.Output:
                        position = ExecuteOutput(position, input);
                        break;
                    case Instruction.GotoIfTrue:
                        position = ExecuteGoto(position, input, true);
                        break;
                    case Instruction.GotoIfFalse:
                        position = ExecuteGoto(position, input, false);
                        break;
                    case Instruction.IsLessThan:
                        position = Execute2Params(position, input, (x, y) => x < y ? 1 : 0);
                        break;
                    case Instruction.Equals:
                        position = Execute2Params(position, input, (x, y) => x == y ? 1 : 0);
                        break;
                    case Instruction.SetBaseAddress:
                        position = ExecuteSetBaseAddress(position, input);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown instruction {currentInstruction}"); ;
                }
            } while (!_quit);
        }

        public void AddInput(long value)
        {
            InputQueue.Enqueue(value);
            TryReleaseSemaphore();
        }

        private int ExecuteGoto(int position, long input, bool compareTo)
        {
            var x = GetParameter(position + 1, GetParameterMode(input, 0));
            var y = GetParameter(position + 2, GetParameterMode(input, 1));
            if ((x != 0) == compareTo) return checked((int)y);
            return position + 3;
        }

        private int Execute2Params(int position, long input, Func<long, long, long> operation)
        {
            var x = GetParameter(position + 1, GetParameterMode(input, 0));
            var y = GetParameter(position + 2, GetParameterMode(input, 1));
            var result = operation(x, y);
            var outputPosition = GetPosition(position + 3, GetParameterMode(input, 2));
            WriteValue(outputPosition, result);
            return position + 4;
        }

        private async Task<int> ExecuteInput(int position, long input)
        {
            long value;
            while (!InputQueue.TryDequeue(out value))
            {
                await _semaphoreSlim.WaitAsync(100).ConfigureAwait(false);
            }
            TryReleaseSemaphore();
            var inputPosition = GetPosition(position + 1, GetParameterMode(input, 0));
            WriteValue(inputPosition, value);
            return position + 2;
        }

        private int ExecuteOutput(int position, long input)
        {
            var outputPosition = position + 1;
            var x = GetParameter(outputPosition, GetParameterMode(input, 0));
            OutputQueue.Enqueue(x);
            OutputProduced?.Invoke(this, new OutputProducedEventArgs(x));
            return position + 2;
        }

        private int ExecuteSetBaseAddress(int position, long input)
        {
            var x = GetParameter(position + 1, GetParameterMode(input, 0));
            _baseAddress += checked((int)x);
            return position + 2;
        }

        private ParameterMode GetParameterMode(long input, int parameterNumber)
        {
            return (ParameterMode)(input / (int)Math.Pow(10, parameterNumber + 2) % 10);
        }

        private long GetParameter(int position, ParameterMode mode)
        {
            position = GetPosition(position, mode);
            return _workingSet[position];
        }

        private int GetPosition(int position, ParameterMode mode)
        {
            switch (mode)
            {
                case ParameterMode.Immediate:
                    break;
                case ParameterMode.Position:
                    position = checked((int)_workingSet[position]);
                    break;
                case ParameterMode.Relative:
                    position = checked(_baseAddress + (int)_workingSet[position]);
                    break;
                default:
                    throw new ArgumentException($"Invalid mode {mode}");
            }
            EnsureWorkingSetSize(position);
            return position;
        }

        private void WriteValue(int position, long value)
        {
            EnsureWorkingSetSize(position);
            _workingSet[position] = value;
        }

        private void EnsureWorkingSetSize(int position)
        {
            if (position >= _workingSet.Length)
            {
                var newSize = checked((int)Math.Pow(2, Math.Ceiling(Math.Log2(position))));
                var newWorkingSet = new long[newSize];
                _workingSet.CopyTo(newWorkingSet, 0);
                Interlocked.Exchange(ref _workingSet, newWorkingSet);
            }
        }

        private void TryReleaseSemaphore()
        {
            try
            {
                if (_semaphoreSlim.CurrentCount == 0)
                    _semaphoreSlim.Release();
            }
            catch { }
        }
    }
}
