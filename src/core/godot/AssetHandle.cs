using Godot;
using Leopotam.Ecs;

public struct AssetHandle<T> : IEcsAutoReset<AssetHandle<T>> where T : Resource
{
    public T Asset;

    public AssetHandle(T node)
    {
        Asset = node;
    }

    public void AutoReset(ref AssetHandle<T> c)
    {
        c.Asset = null;
    }
}