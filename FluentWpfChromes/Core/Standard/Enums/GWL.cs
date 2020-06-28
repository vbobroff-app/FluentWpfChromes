using System.Diagnostics.CodeAnalysis;

namespace FluentWpfChromes
{
    /// <summary>
    /// GetWindowLongPtr values, GWL_*
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    internal enum GWL
    {
        WNDPROC = (-4),
        HINSTANCE = (-6),
        HWNDPARENT = (-8),
        STYLE = (-16),
        EXSTYLE = (-20),
        USERDATA = (-21),
        ID = (-12)
    }
}
