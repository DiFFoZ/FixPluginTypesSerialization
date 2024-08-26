using System;
using System.Collections.Generic;
using FixPluginTypesSerialization.UnityPlayer.Structs.Default;
using FixPluginTypesSerialization.Util;

namespace FixPluginTypesSerialization.UnityPlayer.Structs.v2022.v3
{
    [ApplicableToUnityVersionsSince("2022.3.0")]
    public class MonoManager : IMonoManager
    {
        public MonoManager()
        {
        }

        public MonoManager(IntPtr pointer)
        {
        }

        public IntPtr Pointer { get => CommonUnityFunctions.ScriptingAssemblies; set { } }

        private unsafe RuntimeStatic<ScriptingAssemblies>* _this => (RuntimeStatic<ScriptingAssemblies>*)Pointer;

        private ScriptingAssemblies _originalScriptingAssemblies;

        public List<StringStorageDefaultV3> ManagedAssemblyList = new();
        public int AssemblyCount => ManagedAssemblyList.Count;

        public unsafe void CopyNativeAssemblyListToManaged()
        {
            MonoManagerCommon.CopyNativeAssemblyListToManagedV4(ManagedAssemblyList, _this->value->names);
        }

        public void AddAssembliesToManagedList(List<string> pluginAssemblyPaths)
        {
            MonoManagerCommon.AddAssembliesToManagedListV4(ManagedAssemblyList, pluginAssemblyPaths);
        }

        public unsafe void AllocNativeAssemblyListFromManaged()
        {
            MonoManagerCommon.AllocNativeAssemblyListFromManagedV4(ManagedAssemblyList, &_this->value->names);
        }

        public unsafe void PrintAssemblies()
        {
            MonoManagerCommon.PrintAssembliesV4(_this->value->names);
        }

        public unsafe void RestoreOriginalAssemblyNamesArrayPtr()
        {
            *_this->value = _originalScriptingAssemblies;
        }
    }
}
