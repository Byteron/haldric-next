using Godot;

namespace Haldric;

public class Chunk
{
    public TerrainMesh Mesh;
    public TerrainCollider Collider;
    public TerrainProps Props;

    public bool IsDirty = false;
}