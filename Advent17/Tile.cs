using System;

namespace Advent17
{
    [Flags]
    internal enum Tile
    {
        Empty = 0x2E,
        Path = 0x23,
        BotUp = 0x5E,
        BotRight = 0x3E,
        BotLeft = 0x3C,
        BotDown = 0x76,
        EOL = 0x0A
    }
}