using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FluentWpfChromes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        private int _left;
        private int _top;
        private int _right;
        private int _bottom;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void Offset(int dx, int dy)
        {
            _left += dx;
            _top += dy;
            _right += dx;
            _bottom += dy;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Left
        {
            get => _left;
            set => _left = value;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Right
        {
            get => _right;
            set => _right = value;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Top
        {
            get => _top;
            set => _top = value;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Bottom
        {
            get => _bottom;
            set => _bottom = value;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Width => _right - _left;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Height => _bottom - _top;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public POINT Position => new POINT { x = _left, y = _top };

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public SIZE Size => new SIZE { cx = Width, cy = Height };

        public static RECT Union(RECT rect1, RECT rect2)
        {
            return new RECT
            {
                Left = Math.Min(rect1.Left, rect2.Left),
                Top = Math.Min(rect1.Top, rect2.Top),
                Right = Math.Max(rect1.Right, rect2.Right),
                Bottom = Math.Max(rect1.Bottom, rect2.Bottom),
            };
        }

        public override bool Equals(object obj)
        {
            try
            {
                var rc = (RECT?) obj ?? new RECT();
                return rc._bottom == _bottom
                    && rc._left == _left
                    && rc._right == _right
                    && rc._top == _top;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return (_left << 16 | Utility.LoWord(_right)) ^ (_top << 16 | Utility.LoWord(_bottom));
        }
    }
}
