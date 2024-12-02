using System;
using System.IO;
using BepInEx.Bootstrap;
using HarmonyLib;

namespace FixPluginTypesSerialization;
[HarmonyPatch]
internal static class Patcher
{
    [HarmonyPatch(typeof(Chainloader), nameof(Chainloader.Initialize))]
    [HarmonyPostfix]
    public static void ChainloaderInitialized()
    {
        // BepInEx is ready to load plugins, patching Unity assetbundles

        Log.Init();

        try
        {
            FixPluginTypesSerializationPatcher.InitializeInternal();
        }
        catch (Exception e)
        {
            Log.Error($"Failed to initialize plugin types serialization fix: ({e.GetType()}) {e.Message}. Some plugins may not work properly.");
            Log.Error(e);
        }
    }
}
