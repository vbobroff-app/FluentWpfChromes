using System.Runtime.InteropServices;
using System.Windows;

namespace FluentWpfChromes
{
    internal static class CursorPosition
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PointInter
        {
            public int X;
            public int Y;
            public static explicit operator Point(PointInter point) => new Point(point.X, point.Y);
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out PointInter lpPoint);

        // For your convenience
        public static Point GetCursorPosition()
        {
            GetCursorPos(out var lpPoint);
            return (Point)lpPoint;
        }
    }
}
