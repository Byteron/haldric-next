using RelEcs;
using Godot;

public class EditorState : IState
{
    // public override void Init()
    // {
    //     Enter.Add(new EnterSystem());
    //     Exit.Add(new ExitSystem());
    //
    //     Update
    //         .Add(new UpdateSystem())
    //         .Add(new EditorEditTerrainSystem())
    //         .Add(new EditorSystems())
    //         .Add(new CameraSystems())
    //         .Add(new UpdateHoveredTileSystem())
    //         .Add(new UpdateMapCursorSystem())
    //         .Add(new DebugSystems());
    // }

    public void Enable(World world)
    {
        var data = world.GetElement<TerrainData>();
        
        world.AddElement(new SelectedTerrain
        {
            Entity = data.TerrainEntities["Gg"]
        });
        
        SpawnEditorMenu(world);
        SpawnCamera(world);
        
        SpawnSchedule(world, "DefaultSchedule", 1);
        
        SpawnMap(world, 32, 32);
        UpdateTerrainMesh(world);
        UpdateTerrainProps(world);
    }

    public void Update(World world)
    {
    }

    public void Disable(World world)
    {
        DespawnCamera(world);
        DespawnEditorMenu(world);
        
        world.RemoveElement<SelectedTerrain>();
        
        world.GetElement<Schedule>().QueueFree();
        world.RemoveElement<Schedule>();
    }
}