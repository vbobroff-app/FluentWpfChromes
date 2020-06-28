using System;

namespace FluentWpfChromes
{
    /// <summary>
    /// Shadow Window Accent
    /// </summary>
    [Flags]
    internal enum AccentFlags
    {
        None = 0,
        DrawLeftBorder = 0x20,
        DrawTopBorder = 0x40,
        DrawRightBorder = 0x80,
        DrawBottomBorder = 0x100,
        DrawAllBorders = (DrawLeftBorder | DrawTopBorder | DrawRightBorder | DrawBottomBorder)
    }
}
