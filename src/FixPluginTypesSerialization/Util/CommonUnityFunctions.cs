using System;
using System.Runtime.InteropServices;
using System.Text;
using FixPluginTypesSerialization.UnityPlayer;

namespace FixPluginTypesSerialization.Util
{
    internal class CommonUnityFunctions
    {
        public enum AllocateOptions : int
        {
            None = 0,
            NullIfOutOfMemory
        };

        public struct MemLabelId
        {
            public int id;
            public nint rootref; // only used in debug assembly
        }

        private unsafe delegate IntPtr MallocInternalFunc(ulong size, ulong align, MemLabelId* label, AllocateOptions allocateOptions, IntPtr file, int line);
        private static MallocInternalFunc mallocInternal;

        private delegate void FreeAllocInternalV1Func(IntPtr ptr, int label);
        private unsafe delegate void FreeAllocInternalV2Func(IntPtr ptr, MemLabelId* label, IntPtr file, int line);
        private static FreeAllocInternalV1Func freeAllocInternalV1;
        private static FreeAllocInternalV2Func freeAllocInternalV2;

        public static IntPtr ScriptingAssemblies { get; private set; }

        public static void Init(PatternDiscoverer patternDiscoverer)
        {
            var mallocInternalAddress = patternDiscoverer.Discover(
                Config.MallocInternalOffset,
                [Encoding.ASCII.GetBytes("malloc_internal")]);
            if (mallocInternalAddress != IntPtr.Zero)
            {
                mallocInternal = (MallocInternalFunc)Marshal.GetDelegateForFunctionPointer(mallocInternalAddress, typeof(MallocInternalFunc));
            }

            var freeAllocInternalAddress = patternDiscoverer.Discover(
                Config.FreeAllocInternalOffset,
                [Encoding.ASCII.GetBytes("free_alloc_internal")]);
            if (freeAllocInternalAddress != IntPtr.Zero)
            {
                if (UseRightStructs.UnityVersion >= new Version(2019, 3))
                {
                    freeAllocInternalV2 = (FreeAllocInternalV2Func)Marshal.GetDelegateForFunctionPointer(freeAllocInternalAddress, typeof(FreeAllocInternalV2Func));
                }
                else
                {
                    freeAllocInternalV1 = (FreeAllocInternalV1Func)Marshal.GetDelegateForFunctionPointer(freeAllocInternalAddress, typeof(FreeAllocInternalV1Func));
                }
            }

            var scriptingAssembliesAddress = patternDiscoverer.Discover(
                Config.ScriptingAssembliesOffset,
                [Encoding.ASCII.GetBytes("m_ScriptingAssemblies@")]);
            if (scriptingAssembliesAddress != IntPtr.Zero)
            {
                ScriptingAssemblies = scriptingAssembliesAddress;
            }
        }

        public unsafe static IntPtr MallocString(string str, int label, out ulong length)
        {
            //I couldn't for the life of me find how to adequately convert c# string to ANSI and fill existing pointer
            //that we would get from mallocInternal from c# so we're doing it this way
            var strPtr = Marshal.StringToHGlobalAnsi(str);

            length = (ulong)str.Length;
            //Ansi string might be longer than managed
            for (var c = (byte*)strPtr + length; *c != 0; c++, length++) { }

            var labelStr = new MemLabelId
            {
                id = label,
                rootref = IntPtr.Zero
            };

            var allocPtr = mallocInternal(length + 1, 0x10, &labelStr, AllocateOptions.NullIfOutOfMemory, IntPtr.Zero, 0);

            for (var i = 0ul; i <= length; i++)
            {
                ((byte*)allocPtr)[i] = ((byte*)strPtr)[i];
            }

            Marshal.FreeHGlobal(strPtr);

            return allocPtr;
        }

        public static unsafe void FreeAllocInternal(IntPtr ptr, int label)
        {
            var labelId = new MemLabelId
            {
                id = label,
                rootref = IntPtr.Zero
            };

            if (UseRightStructs.UnityVersion >= new Version(2019, 3))
            {
                freeAllocInternalV2(ptr, &labelId, IntPtr.Zero, 0);
            }
            else
            {
                freeAllocInternalV1(ptr, label);
            }
        }
    }
}
