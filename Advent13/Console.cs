using System.Collections.Generic;

namespace Advent13
{
    internal class Console
    {
        public long Score { get; private set; }
        private (int x, int y) _currentPosition = (0, 0);
        private long _paddleX;
        private long _ballX;

        public void ProcessorOnOutputProduced(object sender, OutputProducedEventArgs e)
        {
            var processor = (Processor)sender;
            if (processor.OutputQueue.Count < 3) return;
            processor.OutputQueue.TryDequeue(out var x);
            processor.OutputQueue.TryDequeue(out var y);
            processor.OutputQueue.TryDequeue(out var t);
            if (x == -1 && y == 0)
            {
                Score = t;
                return;
            }
            if (!PaintedTiles.TryGetValue(_currentPosition, out _))
            {
                PaintedTiles.Add(((int)x, (int)y), (Tile)t);
            }
            else
            {
                PaintedTiles[((int)x, (int)y)] = (Tile)t;
            }
            switch ((Tile)t)
            {
                case Tile.Ball:
                    _ballX = x;
                    processor.AddInput(_ballX.CompareTo(_paddleX));
                    break;
                case Tile.Paddle:
                    _paddleX = x;
                    break;
            }
        }

        public Dictionary<(int x, int y), Tile> PaintedTiles { get; } = new Dictionary<(int x, int y), Tile>();
    }
}