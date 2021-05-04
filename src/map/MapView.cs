using Godot;
using Leopotam.Ecs;

public partial class MapView : Node3D
{
    TerrainChunk _chunk;

    public override void _Ready()
    {
        _chunk = GetNode<TerrainChunk>("TerrainChunk");
    }

    public void Build(Locations locations)
    {
        GD.Print("Building Map!: ", locations.Count);
        
        _chunk.Build(locations);
    }
}
