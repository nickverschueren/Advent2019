using System;

namespace Advent15
{
    public class OutputProducedEventArgs : EventArgs
    {
        public OutputProducedEventArgs(long output)
        {
            Output = output;
        }

        public long Output { get; }
    }
}