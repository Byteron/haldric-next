using Godot;
using RelEcs;
using RelEcs.Godot;

public struct AssetHandle<T> : IReset<AssetHandle<T>> where T : Resource
{
    public T Asset { get; set; }

    public AssetHandle(T asset)
    {
        Asset = asset;
    }

    public void Reset(ref AssetHandle<T> c)
    {
        c.Asset = null;
    }
}