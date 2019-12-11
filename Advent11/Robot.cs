using System.Collections.Generic;

namespace Advent11
{
    internal class Robot
    {
        private readonly Dictionary<(int x, int y), bool> _paintedTiles = new Dictionary<(int x, int y), bool>();
        private (int x, int y) _currentPosition = (0, 0);
        private Orientation _currentOrientation = Orientation.Up;

        public void ProcessorOnOutputProduced(object sender, OutputProducedEventArgs e)
        {
            var processor = (Processor)sender;
            if (processor.OutputQueue.Count < 2) return;
            processor.OutputQueue.TryDequeue(out var color);
            processor.OutputQueue.TryDequeue(out var turn);
            if (!_paintedTiles.TryGetValue(_currentPosition, out _))
            {
                _paintedTiles.Add(_currentPosition, color != 0);
            }
            else
            {
                _paintedTiles[_currentPosition] = color != 0;
            }

            _currentOrientation = (Orientation)(((int)_currentOrientation + (turn == 0 ? 3 : 1)) % 4);
            switch (_currentOrientation)
            {
                case Orientation.Up:
                    _currentPosition = (_currentPosition.x, _currentPosition.y + 1);
                    break;
                case Orientation.Right:
                    _currentPosition = (_currentPosition.x + 1, _currentPosition.y);
                    break;
                case Orientation.Down:
                    _currentPosition = (_currentPosition.x, _currentPosition.y - 1);
                    break;
                case Orientation.Left:
                    _currentPosition = (_currentPosition.x - 1, _currentPosition.y);
                    break;
            }

            processor.AddInput(_paintedTiles.TryGetValue(_currentPosition, out var currentColor) ? (currentColor ? 1 : 0) : 0);
        }

        public Dictionary<(int x, int y), bool> PaintedTiles => _paintedTiles;
    }
}