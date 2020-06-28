using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;

namespace FluentWpfChromes
{
    internal static class Utility
    {

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static int GetXlParam(IntPtr lParam)
        {
            return LoWord(lParam.ToInt32());
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static int GetYlParam(IntPtr lParam)
        {
            return HiWord(lParam.ToInt32());
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static int HiWord(int i)
        {
            return (short)(i >> 16);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static int LoWord(int i)
        {
            return (short)(i & 0xFFFF);
        }

        public static bool IsThicknessNonNegative(Thickness thickness)
        {
            if (!IsDoubleFiniteAndNonNegative(thickness.Top))
            {
                return false;
            }

            if (!IsDoubleFiniteAndNonNegative(thickness.Left))
            {
                return false;
            }

            if (!IsDoubleFiniteAndNonNegative(thickness.Bottom))
            {
                return false;
            }

            if (!IsDoubleFiniteAndNonNegative(thickness.Right))
            {
                return false;
            }

            return true;
        }

        public static bool IsDoubleFiniteAndNonNegative(double d)
        {
            return !double.IsNaN(d) && !double.IsInfinity(d) && !(d < 0);
        }

        /// <summary>
        /// Compare object values that may be null
        /// </summary>
        /// <param name="value"></param>
        /// <param name="eqValue"></param>
        /// <returns></returns>
        public static bool IsEqual(this object value, object eqValue)
        {
            return value?.Equals(eqValue) ?? eqValue == null;
        }

        /// <summary>
        /// Convert string to double according culture symbols
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToDouble(this string value)
        {
            // Try parsing in the current culture
            if (
                !double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out var result) &&
                // Then try in US english
                !double.TryParse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"),
                    out result) &&
                // Then in neutral language
                !double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                throw new ArgumentException();
            }

            return result;

        }

        public static double RealHeight(this FrameworkElement fe)
        {
            return  double.IsNaN(fe.Height) ? fe.ActualHeight : fe.Height;
        }

    }
}
