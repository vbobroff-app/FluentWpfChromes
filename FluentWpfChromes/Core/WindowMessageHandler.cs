using System;

namespace FluentWpfChromes
{
    internal delegate IntPtr WindowMessageHandler(IntPtr hWnd, WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled);
}
