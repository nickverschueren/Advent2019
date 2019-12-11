using System;

namespace Advent11
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