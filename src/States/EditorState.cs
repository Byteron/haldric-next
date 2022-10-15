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
        
        world.SpawnEditorMenu();
        world.SpawnCamera();
        
        world.SpawnSchedule("DefaultSchedule", 1);
        
        world.SpawnMap(32, 32);
        world.UpdateTerrainMesh();
        world.UpdateTerrainProps();
    }

    public void Update(World world)
    {
    }

    public void Disable(World world)
    {
        world.DespawnCamera();
        world.DespawnEditorMenu();
        
        world.RemoveElement<SelectedTerrain>();
        
        world.GetElement<Schedule>().QueueFree();
        world.RemoveElement<Schedule>();
    }
}