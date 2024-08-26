using System.Runtime.InteropServices;

namespace FixPluginTypesSerialization.UnityPlayer.Structs.Default
{
    // struct dynamic_array_detail::dynamic_array_data
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct DynamicArrayData
    {
        public nint ptr;
        public int label;
        public nint labelRootRef; // only exists on dev builds
        public ulong size;
        public ulong capacity;
    }
}
