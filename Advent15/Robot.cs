using System;
using System.Collections.Generic;

namespace Advent15
{
    internal class Robot
    {
        private (int x, int y) _currentPosition = (0, 0);
        private Orientation _currentOrientation = Orientation.North;
        public int Moves { get; private set; } = 0;
        public bool ByTheRight { get; set; } = true;
        public bool Explore { get; set; } = false;

        public void ProcessorOnOutputProduced(object sender, OutputProducedEventArgs e)
        {
            var processor = (Processor)sender;
            processor.OutputQueue.TryDequeue(out var x);
            switch (x)
            {
                case 0:
                    var wall = Move(_currentPosition, _currentOrientation);
                    if (!PaintedTiles.ContainsKey(wall))
                        PaintedTiles.Add(wall, Tile.Wall);
                    _currentOrientation = ByTheRight ? TurnLeft(_currentOrientation) : TurnRight(_currentOrientation);
                    break;
                case 1:
                    _currentPosition = Move(_currentPosition, _currentOrientation);
                    if (!PaintedTiles.ContainsKey(_currentPosition))
                    {
                        PaintedTiles.Add(_currentPosition, Tile.Visited);
                        Moves++;
                    }
                    else
                    {
                        Moves--;
                    }
                    _currentOrientation = ByTheRight ? TurnRight(_currentOrientation) : TurnLeft(_currentOrientation);
                    break;
                case 2:
                    _currentPosition = Move(_currentPosition, _currentOrientation);
                    if (!PaintedTiles.ContainsKey(_currentPosition))
                        PaintedTiles.Add(_currentPosition, Tile.Oxigen);
                    Moves++;
                    if (!Explore) processor.Quit();
                    break;
            }
            if (Explore &&  _currentOrientation == Orientation.North && _currentPosition == (0, 0)) processor.Quit();
            processor.AddInput((int)_currentOrientation);
        }

        private Orientation TurnRight(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.North:
                    return Orientation.East;
                case Orientation.East:
                    return Orientation.South;
                case Orientation.South:
                    return Orientation.West;
                default:
                    return Orientation.North;
            }
        }

        private Orientation TurnLeft(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.North:
                    return Orientation.West;
                case Orientation.East:
                    return Orientation.North;
                case Orientation.South:
                    return Orientation.East;
                default:
                    return Orientation.South;
            }
        }

        private (int x, int y) Move((int x, int y) position, Orientation direction)
        {
            switch (direction)
            {
                case Orientation.North:
                    return (position.x, position.y - 1);
                case Orientation.South:
                    return (position.x, position.y + 1);
                case Orientation.East:
                    return (position.x + 1, position.y);
                default:
                    return (position.x - 1, position.y);
            }
        }

        public Dictionary<(int x, int y), Tile> PaintedTiles { get; } = new Dictionary<(int x, int y), Tile>();
    }
}