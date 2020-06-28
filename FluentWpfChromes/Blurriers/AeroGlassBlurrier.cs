using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace FluentWpfChromes
{
    /// <summary>
    /// Set Enable/Dissamble Blur effect on Window
    /// for OC Windows 8-10
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal class AeroGlassBlurrier
    {

        /// <summary>
        /// Enable blur by SetAccentPolicy
        /// </summary>
        /// <param name="hWnd">
        /// Current Window handle
        /// </param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void EnableBlur(IntPtr hWnd)
        {
            if (SystemParameters.HighContrast)
            {
                return; // Blur is not useful in high contrast mode 
            }

            var hex = (uint) UsingColors.Transparent;
            NativeMethods.SetAccentPolicy(hWnd, AccentState.ACCENT_ENABLE_BLURBEHIND, AccentFlags.None, hex);

        }

        #region EaanbleBlur overloads

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void EnableBlur(IntPtr hWnd, uint hex)
        {
            if (SystemParameters.HighContrast)
            {
                return; // Blur is not useful in high contrast mode 
            }

            NativeMethods.SetAccentPolicy(hWnd, AccentState.ACCENT_ENABLE_BLURBEHIND, AccentFlags.None, hex);

        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void EnableBlur(IntPtr hWnd, AccentState accentState, AccentFlags accentFlags, uint hex)
        {
            if (SystemParameters.HighContrast)
            {
                return; // Blur is not useful in high contrast mode 
            }

            NativeMethods.SetAccentPolicy(hWnd, accentState, accentFlags, hex);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void EnableBlur(IntPtr hWnd, AccentState accentState, uint hex)
        {
            if (SystemParameters.HighContrast)
            {
                return; // Blur is not useful in high contrast mode 
            }

            NativeMethods.SetAccentPolicy(hWnd, accentState, AccentFlags.None, hex);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void EnableBlur(IntPtr hWnd, AccentFlags accentFlags, uint hex)
        {
            if (SystemParameters.HighContrast)
            {
                return; // Blur is not useful in high contrast mode 
            }

            NativeMethods.SetAccentPolicy(hWnd, AccentState.ACCENT_ENABLE_BLURBEHIND, accentFlags, hex);

        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void EnableBlur(IntPtr hWnd, AccentFlags accentFlags)
        {
            if (SystemParameters.HighContrast)
            {
                return; // Blur is not useful in high contrast mode 
            }

            var hex = (uint) UsingColors.Transparent;
            NativeMethods.SetAccentPolicy(hWnd, AccentState.ACCENT_ENABLE_BLURBEHIND, accentFlags, hex);

        }

        #endregion

        /// <summary>
        /// Disable blur by SetAccentPolicy
        /// </summary>
        /// <param name="hWnd">
        /// Current Window handle
        /// </param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void DisableBlur(IntPtr hWnd)
        {
            var hex = (uint) UsingColors.Transparent;
            NativeMethods.SetAccentPolicy(hWnd, AccentState.ACCENT_ENABLE_TRANSPARENTGRADIENT, AccentFlags.None, hex);
        }

        #region DisableBlur overloads

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void DisableBlur(IntPtr hWnd, UsingColors gradientColor)
        {
            var hex = (uint) gradientColor;
            NativeMethods.SetAccentPolicy(hWnd, AccentState.ACCENT_ENABLE_TRANSPARENTGRADIENT, AccentFlags.None, hex);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void DisableBlur(IntPtr hWnd, AccentState accentState, UsingColors gradientColor)
        {
            var hex = (uint) gradientColor;
            NativeMethods.SetAccentPolicy(hWnd, accentState, AccentFlags.None, hex);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void DisableBlur(IntPtr hWnd, AccentState accentState, AccentFlags flags,
            UsingColors gradientColor)
        {
            var hex = (uint) gradientColor;
            NativeMethods.SetAccentPolicy(hWnd, accentState, flags, hex);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void DisableBlur(IntPtr hWnd, uint hex)
        {
            NativeMethods.SetAccentPolicy(hWnd, AccentState.ACCENT_ENABLE_TRANSPARENTGRADIENT, AccentFlags.None, hex);
        }

        #endregion
    }
}
