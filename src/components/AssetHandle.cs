using Godot;
using Bitron.Ecs;

public struct AssetHandle<T> : IEcsAutoReset<AssetHandle<T>> where T : Resource
{
    public T Asset { get; set; }

    public AssetHandle(T asset)
    {
        Asset = asset;
    }

    public void AutoReset(ref AssetHandle<T> c)
    {
        c.Asset = null;
    }
}