namespace FixPluginTypesSerialization.UnityPlayer.Structs.Default
{
    public unsafe struct RuntimeStatic<T> where T : unmanaged
    {
        public T* value;
    }
}
