using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;

namespace FluentWpfChromes
{
    /// <summary>
    /// Extensions for Color Type
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    internal static class ColorFrom
    {
        public static Color SolidBrush(SolidColorBrush brush)
        {
            return brush.Color;
        }

        public static Color Zero = new Color();

        public static Color BlackOpacity = Color.FromArgb(1, 0, 0, 0);  

        public static Color Hex(string hex)
        {
            var hc = ColorConverter.ConvertFromString(hex);
            if (hc != null) return (Color)hc;
            return Colors.Transparent;
        }

        public static Color String(string color)
        {
            if (color == "") return Colors.Transparent;
            if (color == null) return Color.FromArgb(0, 0, 0, 0);

            var c = ColorConverter.ConvertFromString(color);

            if (c != null) return (Color)c;
            return Color.FromArgb(0, 0, 0, 0);
        }

        public static Color Object(object color)
        {
            var c = 0x00000000.ToColor();

            if (color == null) return c;

            try
            {
                c = (Color)color;
            }
            catch (Exception)
            {

                try
                {
                    c = ((int)color).ToColor();
                }
                catch (Exception)
                {
                    try
                    {
                        c = ((uint)color).ToColor();
                    }
                    catch (Exception)
                    {

                        try
                        {
                            c = ((long)color).ToColor();
                        }
                        catch (Exception)
                        {
                            try
                            {
                                c = ColorFrom.String((string)color);
                            }
                            catch (Exception e)
                            {
                                throw new InvalidOperationException("Color convert Error", e);
                            }
                        }


                    }
                }

            }

            return c;
        }

        /// <summary>
        /// According CS3002 CLS-Compliant, internal
        /// </summary>
        /// <returns></returns>
        internal static Color HexArgb(uint hex)
        {
            return Color.FromArgb(//hex>0xFFFFFF?
                (byte)((hex & 0xff000000) >> 0x18),//:(byte)0xFF,
                (byte)((hex & 0xff0000) >> 0x10),
                (byte)((hex & 0xff00) >> 8),
                (byte)(hex & 0xff));
        }

        /// <summary>
        /// According CS3002 CLS-Compliant, public
        /// </summary>
        /// <param name="hexL">
        /// HexL ARGB data, f.e. 0xFF000000L
        /// </param>
        /// <returns></returns>
        public static Color HexLArgb(long hexL)
        {
            return Color.FromArgb(//hexL > 0xFFFFFF ?
                (byte)((hexL & 0xff000000L) >> 0x18),//:(byte)0xFF,
                (byte)((hexL & 0xff0000L) >> 0x10),
                (byte)((hexL & 0xff00L) >> 8),
                (byte)(hexL & 0xffL));
        }

        /// <summary>
        /// According CS3002 CLS-Compliant, internal
        /// </summary>
        /// <returns></returns>
        internal static Color HexAbgr(uint hex)
        {
            return Color.FromArgb(//hex > 0xFFFFFF ?
            //    (byte)((hex & 0xff000000) >> 0x18) : 
            (byte)0xFF,
                (byte)(hex & 0xff),
                (byte)((hex & 0xff00) >> 8),
                (byte)((hex & 0xff0000) >> 0x10));
        }

        /// <summary>
        /// According CS3002 CLS-Compliant, public
        /// </summary>
        /// <param name="hexL">
        /// HexL ABGR data, f.e. 0xFF000000L
        /// </param>
        /// <returns></returns>
        public static Color HexAbgrL(long hexL)
        {
            return Color.FromArgb(//hexL > 0xFFFFFF ?
                (byte)((hexL & 0xff000000L) >> 0x18),// : (byte) 0xFF,
                (byte)(hexL & 0xffL),
                (byte)((hexL & 0xff00L) >> 8),
                (byte)((hexL & 0xff0000L) >> 0x10));
        }

        /// <summary>
        /// According CS3002 CLS-Compliant, internal
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static uint ToARGBhex(this Color c)
        {
            return (uint)(((c.A << 24) | (c.R << 16) | (c.G << 8) | c.B) & 0xffffffff);
        }

        /// <summary>
        /// According CS3002 CLS-Compliant, public
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static long ToARGBhexL(this Color c)
        {

            return (((c.A << 24) | (c.R << 16) | (c.G << 8) | c.B) & 0xffffffffL);

        }

        /// <summary>
        /// According CS3002 CLS-Compliant, internal
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static uint ToABGRhex(this Color c)
        {
            return (uint)(((c.A << 0x18) | (c.B << 0x10) | (c.G << 8) | c.R) & 0xFFFFFFFF);
        }

        /// <summary>
        /// According CS3002 CLS-Compliant, public
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static long ToABGRhexL(this Color c)
        {
            return (((c.A << 0x18) | (c.B << 0x10) | (c.G << 8) | c.R) & 0xFFFFFFFFL);
        }

        /// <summary>
        /// According CS3002 CLS-Compliant, internal
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static uint ToRGBhex(this Color c)
        {
            return (uint)(((c.R << 16) | (c.G << 8) | c.B) & 0xffffffff);
        }

        /// <summary>
        /// According CS3002 CLS-Compliant, public
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static long ToRGBhexL(this Color c)
        {
            return (((c.R << 16) | (c.G << 8) | c.B) & 0xffffffffL);
        }

        /// <summary>
        /// According CS3002 CLS-Compliant, internal
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static uint ToBGRhex(this Color c)
        {
            return (uint)(((c.B << 16) | (c.G << 8) | c.R) & 0xffffffff);
        }

        /// <summary>
        /// According CS3002 CLS-Compliant, public
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static long ToBGRhexL(this Color c)
        {
            return (((c.B << 16) | (c.G << 8) | c.R) & 0xffffffffL);
        }

        /// <summary>
        /// Hex uint  to ARGB Color 
        /// </summary>
        /// <param name="hex">
        /// Hex ARGB data, f.e. 0xFF000000
        /// </param>
        /// <returns></returns>
        internal static Color ToColor(this uint hex)
        {
            return Color.FromArgb(//hex > 0xFFFFFF ?
            //    (byte)((hex & 0xff000000) >> 0x18) : 
            (byte)0xFF,
                (byte)((hex & 0xff0000) >> 0x10),
                (byte)((hex & 0xff00) >> 8),
                (byte)(hex & 0xff));
        }

        /// <summary>
        /// Hex long  to ARGB Color 
        /// </summary>
        /// <param name="hexL">
        /// HexL ARGB data, f.e. 0xFF000000L
        /// </param>
        /// <returns></returns>
        public static Color ToColor(this long hexL)
        {
            return Color.FromArgb(//hexL > 0xFFFFFF ?
                (byte)((hexL & 0xff000000L) >> 0x18),// : (byte) 0xFF,
                (byte)((hexL & 0xff0000L) >> 0x10),
                (byte)((hexL & 0xff00L) >> 8),
                (byte)(hexL & 0xffL));
        }

        /// <summary>
        /// Hex uint  to ARGB Color 
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color ToColor(this int hex)
        {
            return Color.FromArgb(//hex > 0xFFFFFF ?
            //    (byte)((hex & 0xff000000) >> 0x18) : 
            (byte)0xFF,
                (byte)((hex & 0xff0000) >> 0x10),
                (byte)((hex & 0xff00) >> 8),
                (byte)(hex & 0xff));
        }

        /// <summary>
        /// Get Hex uint RGB channel, f.e. 0xFF112233 -> 0x112233  
        /// </summary>
        /// <param name="x">
        ///  data object Color, Hex, HexL, string
        /// </param>
        /// <returns></returns>
        internal static uint RgbChannel(this object x)
        {
            if (x == null) return 0x000000;

            try
            {
                var c = (Color)x;
                return (uint)(((c.R << 16) | (c.G << 8) | c.B) & 0xffffffL);
            }
            catch (Exception)
            {
                try
                {
                    var i = (int)x;
                    return (uint)(i & 0xffffff);
                }
                catch (Exception)
                {
                    try
                    {
                        var u = (uint)x;
                        return (u & 0xffffff);
                    }
                    catch (Exception)
                    {

                        try
                        {
                            var u = (long)x;
                            return (uint)(u & 0xffffffL);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                var str = (string)x;
                                var c = ColorFrom.String(str);
                                return (uint)(((c.R << 16) | (c.G << 8) | c.B) & 0xffffffL);
                            }
                            catch (Exception e)
                            {
                                throw new InvalidOperationException("Color convert Error", e);
                            }
                        }



                    }
                }

            }



        }

        /// <summary>
        /// Get Hex long RGB channel, f.e. 0xFF112233 -> 0x112233  
        /// </summary>
        /// <param name="x">
        ///  data object Color, Hex, HexL, string
        /// </param>
        /// <returns></returns>
        public static long RgbLChannel(this object x)
        {
            if (x == null) return 0x000000;

            try
            {
                var c = (Color)x;
                return (((c.R << 16) | (c.G << 8) | c.B) & 0xffffffL);
            }
            catch (Exception)
            {
                try
                {
                    var i = (int)x;
                    return (i & 0xffffffL);
                }
                catch (Exception)
                {
                    try
                    {
                        var u = (uint)x;
                        return (u & 0xffffffL);
                    }
                    catch (Exception)
                    {

                        try
                        {
                            var u = (long)x;
                            return (u & 0xffffffL);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                var str = (string)x;
                                var c = ColorFrom.String(str);
                                return (long)(((c.R << 16) | (c.G << 8) | c.B) & 0xffffffL);
                            }
                            catch (Exception e)
                            {
                                throw new InvalidOperationException("Color convert Error", e);
                            }
                        }



                    }
                }

            }



        }

    }
}
