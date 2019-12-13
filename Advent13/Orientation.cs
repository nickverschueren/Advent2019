using System;

namespace Advent13
{
    [Flags]
    internal enum Tile
    {
        Empty = 0,
        Wall = 1,
        Block = 2,
        Paddle = 3,
        Ball = 4
    }
}