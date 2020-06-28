using System.Diagnostics.CodeAnalysis;

namespace FluentWpfChromes
{
    /// <summary>
    /// DWMWINDOWATTRIBUTE.  DWMWA_*
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "CommentTypo")]
    internal enum DWMWA
    {
        NCRENDERING_ENABLED = 1,
        NCRENDERING_POLICY=2,
        TRANSITIONS_FORCEDISABLED,
        ALLOW_NCPAINT,
        CAPTION_BUTTON_BOUNDS,
        NONCLIENT_RTL_LAYOUT,
        FORCE_ICONIC_REPRESENTATION,
        FLIP3D_POLICY,
        EXTENDED_FRAME_BOUNDS,

        // New to Windows 7:

        HAS_ICONIC_BITMAP,
        DISALLOW_PEEK,
        EXCLUDED_FROM_PEEK,

        // LAST
    }
}
