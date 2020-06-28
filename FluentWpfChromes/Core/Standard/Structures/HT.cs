using System.Diagnostics.CodeAnalysis;

namespace FluentWpfChromes
{
    /// <summary>
    /// Non-client hit test values, HT*
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal enum HT
    {
        ERROR = -2,
        TRANSPARENT = -1,
        NOWHERE = 0,
        CLIENT = 1,
        CAPTION = 2,
        SYSMENU = 3,
        GROWBOX = 4,
        SIZE = GROWBOX,
        MENU = 5,
        HSCROLL = 6,
        VSCROLL = 7,
        MINBUTTON = 8,
        MAXBUTTON = 9,
        LEFT = 10,
        RIGHT = 11,
        TOP = 12,
        TOPLEFT = 13,
        TOPRIGHT = 14,
        BOTTOM = 15,
        BOTTOMLEFT = 16,
        BOTTOMRIGHT = 17,
        BORDER = 18,
        REDUCE = MINBUTTON,
        ZOOM = MAXBUTTON,
        SIZEFIRST = LEFT,
        SIZELAST = BOTTOMRIGHT,
        OBJECT = 19,
        CLOSE = 20,
        HELP = 21
    }
}
