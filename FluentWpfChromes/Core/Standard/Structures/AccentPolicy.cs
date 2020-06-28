using System.Runtime.InteropServices;

namespace FluentWpfChromes
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public AccentFlags AccentFlags;
        public uint GradientColor;
        public int AnimationId;
    }
}
