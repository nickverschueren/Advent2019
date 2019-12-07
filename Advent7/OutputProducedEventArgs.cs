using System;

namespace Advent7
{
    public class OutputProducedEventArgs : EventArgs
    {
        public OutputProducedEventArgs(int output)
        {
            Output = output;
        }

        public int Output { get; }
    }
}