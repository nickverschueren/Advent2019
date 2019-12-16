using System;

namespace Advent15
{
    [Flags]
    internal enum Tile
    {
        Empty = 0,
        Visited = 1,
        Wall = 2,
        Oxigen = 3,
        Start = 4
    }
}