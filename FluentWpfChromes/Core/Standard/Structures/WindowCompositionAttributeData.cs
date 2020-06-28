using System;
using System.Runtime.InteropServices;

namespace FluentWpfChromes
{
    /// <summary>
    /// Parameter of SetWindowCompositionAttribute native function
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }
}
