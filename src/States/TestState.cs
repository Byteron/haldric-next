using RelEcs;

public class TestState : IState
{
    public void Enable(World world)
    {
        var scene = world.GetTree().CurrentScene;
        var data = world.GetElement<ScenarioData>();

        var camera = Scenes.Instantiate<CameraOperator>();
        world.AddElement(camera);
        scene.AddChild(camera);

        var mapData = data.Maps["Valley"];
        SpawnSchedule(world, "DefaultSchedule", 0);
        SpawnMap(world, mapData);
        UpdateTerrainMesh(world);
        UpdateTerrainProps(world);

        world.AddElement(new LocalPlayer { Id = 0 });
        world.AddElement(new Scenario());

        var canvas = world.GetElement<Canvas>();
        var canvasLayer = canvas.GetCanvasLayer(1);

        var turnPanel = Scenes.Instantiate<TurnPanel>();
        turnPanel.EndTurnButton.Pressed += () => EndTurn(world);
        canvasLayer.AddChild(turnPanel);
        world.AddElement(turnPanel);

        var unitPanel = Scenes.Instantiate<UnitPanel>();
        canvasLayer.AddChild(unitPanel);
        world.AddElement(unitPanel);

        var terrainPanel = Scenes.Instantiate<TerrainPanel>();
        canvasLayer.AddChild(terrainPanel);
        world.AddElement(terrainPanel);

        SpawnPlayer(world, 0, 0, Coords.FromOffset(1, 1), "Humans", 1000);
        SpawnPlayer(world, 0, 1, Coords.FromOffset(2, 2), "Humans", 1000);

        EndTurn(world);
    }

    public void Update(World world)
    {
        UpdateCamera(world);
        UpdateHoveredTile(world);
        UpdateDebugInfo(world);
        UpdateMapCursor(world);
    }

    public void Disable(World world)
    {
        DespawnMap(world);
        world.GetElement<CameraOperator>().QueueFree();
        world.RemoveElement<CameraOperator>();
        world.GetElement<Schedule>().QueueFree();
        world.RemoveElement<Schedule>();
    }
}