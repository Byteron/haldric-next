using Godot;

namespace Haldric;

public class Chunk
{
    public Vector2I Cell;
    
    public TerrainMesh Mesh;
    public TerrainCollider Collider;
    public TerrainProps Props;
}