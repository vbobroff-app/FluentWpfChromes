using System.Windows;

namespace FluentWpfChromes
{
    public struct MARGINS
    {
        public MARGINS(Thickness t)
        {
            Left = (int)t.Left;
            Right = (int)t.Right;
            Top = (int)t.Top;
            Bottom = (int)t.Bottom;
        }
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }
}
