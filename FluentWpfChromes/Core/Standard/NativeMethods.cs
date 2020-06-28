using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace FluentWpfChromes
{
    /// <summary>
    /// According CA1060: Move P-Invokes to NativeMethods class
    /// </summary>
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    internal static class NativeMethods
    {

        /// <summary>
        /// Using for blurred, and acrylic window 
        /// </summary>
        /// <param name="hWnd">
        /// Window handle
        /// </param>
        /// <param name="data">
        /// Attribute data
        /// </param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowCompositionAttribute")]
        private static extern int _SetWindowCompositionAttribute(IntPtr hWnd, ref WindowCompositionAttributeData data);

        [DllImport("user32.dll", EntryPoint = "SetCapture")]
        public static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        public static extern bool ReleaseCapture();

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
        private static extern int _GetWindowLong32(IntPtr hWnd, GWL nIndex);

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr _GetWindowLongPtr64(IntPtr hWnd, GWL nIndex);

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern int _SetWindowLong32(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr _SetWindowLongPtr64(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "GetWindowRect", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute")]
        private static extern void _DwmSetWindowAttribute(IntPtr hWnd, DWMWA dwAttribute, ref int pvAttribute, int cbAttribute);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "DefWindowProcW")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "PostMessage", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _PostMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        // This function returns a BOOL if TPM_RETURNCMD isn't specified, but otherwise the command Id.
        // Currently it's only used with TPM_RETURNCMD, so making the signature match that.
        [DllImport("user32.dll")]
        public static extern uint TrackPopupMenuEx(IntPtr hMenu, uint fuFlags, int x, int y, IntPtr hWnd, IntPtr lptpm);


        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void PostMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam)
        {
            if (!_PostMessage(hWnd, Msg, wParam, lParam))
            {
                throw new Win32Exception();
            }
        }

        internal static void VistaDisableBlurFrame(IntPtr hwnd)
        {
            var policyParameter = (int) DWMNCRP.DISABLED;
            _DwmSetWindowAttribute(hwnd, DWMWA.NCRENDERING_POLICY, ref policyParameter, sizeof(int));
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr GetWindowLongPtr(IntPtr hWnd, GWL nIndex)
        {
            var res = (8 == IntPtr.Size) 
                      ? _GetWindowLongPtr64(hWnd, nIndex) 
                      : new IntPtr(_GetWindowLong32(hWnd, nIndex));

            if (IntPtr.Zero == res)
            {
                throw new Win32Exception();
            }
            return res;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr SetWindowLongPtr(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong)
        {
            return (8 == IntPtr.Size) 
                    ? _SetWindowLongPtr64(hWnd, nIndex, dwNewLong) 
                    : new IntPtr(_SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static IntPtr GetWindowStyle(IntPtr hWnd)
        {
            return GetWindowLongPtr(hWnd, GWL.STYLE);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static void RemoveTitleBar(IntPtr hWnd)
        {
            //Remove title bar
            var lCurStyle = GetWindowLongPtr(hWnd, GWL.STYLE).ToInt32();  // GWL_STYLE=-16

            lCurStyle &= ~(int)(WS.CAPTION | WS.HSCROLL | WS.VSCROLL | WS.MAXIMIZE);
            lCurStyle &= (int)WS.MAXIMIZE;
            SetWindowLongPtr(hWnd, GWL.STYLE, new IntPtr(lCurStyle));
        }

        internal static void SetInvisibleNonClientArea(IntPtr hWnd)
        {
             SetWindowLongPtr(hWnd, GWL.STYLE, new IntPtr((int)WS.VISIBLE));
             SetWindowLongPtr(hWnd, GWL.STYLE,  new IntPtr((int)WS_EX.MDICHILD));
        }

        internal static void ShowSystemMenuPhysicalCoordinates(IntPtr hWnd, Point physicalScreenLocation)
        {
            const uint TPM_RETURNCMD = 0x0100;
            const uint TPM_LEFTBUTTON = 0x0;
           
            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            var hMenu = GetSystemMenu(hWnd, false);

            var cmd = TrackPopupMenuEx(hMenu, TPM_LEFTBUTTON | TPM_RETURNCMD, (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, hWnd, IntPtr.Zero);
            if (0 != cmd)
            {
                PostMessage(hWnd, WM.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
            }
        }

        internal static void SetAccentPolicy(IntPtr hWnd, AccentState accentState, AccentFlags accentFlags, uint gradientColor)
        {

            var accent = new AccentPolicy
            {
                AccentState = accentState,
                AccentFlags = accentFlags,
                AnimationId = 0,
                GradientColor = gradientColor
            };

            var accentStructSize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };
            

            _SetWindowCompositionAttribute(hWnd, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static MARGINS GetDpiAdjustedMargins(IntPtr hWnd, int left, int right, int top, int bottom)
        {
           // Get system Dpi 
           //var desktop = System.Drawing.Graphics.FromHwnd(windowHandle);
           // var DesktopDpiX = desktop.DpiX;
           // var DesktopDpiY = desktop.DpiY;
           if(hWnd==IntPtr.Zero) throw new ArgumentException();

           var source = HwndSource.FromHwnd(hWnd);
        
            if (source==null) throw new NullReferenceException("source");
            if (source.CompositionTarget == null) throw new NullReferenceException("CompositionTarget");

            var DesktopDpiX = source.CompositionTarget.TransformToDevice.M11;
            var DesktopDpiY = source.CompositionTarget.TransformToDevice.M22;


            // Set Fields
            var margins = new MARGINS
            {
                Left = Convert.ToInt32(left * (DesktopDpiX / 96)),
                Right = Convert.ToInt32(right * (DesktopDpiX / 96)),
                Top = Convert.ToInt32(top * (DesktopDpiY / 96)),
                Bottom = Convert.ToInt32(right * (DesktopDpiY / 96))
            };

            return margins;
        }

        public static RECT GetWindowRect(IntPtr hWnd)
        {
            RECT rc;
            if (!_GetWindowRect(hWnd, out rc))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return rc;
        }

    }
}
