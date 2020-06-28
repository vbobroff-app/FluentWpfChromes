using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace FluentWpfChromes
{
    /// <summary>
    /// Blurrier fo Windows 7 OC
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal class VistaGlassBlurrier
    {
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool ExtendGlassFrame(IntPtr hWnd, Thickness margin)
        {
            if (!NativeMethods.DwmIsCompositionEnabled())
                return false;

            if (hWnd == IntPtr.Zero)
                throw new InvalidOperationException("The Window must be shown before extending glass.");
            var margins = new MARGINS(margin);
            NativeMethods.DwmExtendFrameIntoClientArea(hWnd, ref margins);

            // Set the background to transparent from both the WPF and Win32 perspectives
            // window.Background = Brushes.Transparent;
            var source = HwndSource.FromHwnd(hWnd);
            if (source?.CompositionTarget == null) return false;
            source.CompositionTarget.BackgroundColor = Colors.Transparent;


            NativeMethods.DwmExtendFrameIntoClientArea(hWnd, ref margins);

            return true;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
     
        public static void DisableBlurFrame(IntPtr hWnd)
        {
            NativeMethods.VistaDisableBlurFrame(hWnd);
        }
    }
}
