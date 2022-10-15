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
        world.SpawnSchedule("DefaultSchedule", 0);
        world.SpawnMap(mapData);
        world.UpdateTerrainMesh();
        world.UpdateTerrainProps();

        world.AddElement(new LocalPlayer { Id = 0 });
        world.AddElement(new Scenario());

        var canvas = world.GetElement<Canvas>();
        var canvasLayer = canvas.GetCanvasLayer(1);

        var turnPanel = Scenes.Instantiate<TurnPanel>();
        turnPanel.EndTurnButton.Pressed += world.EndTurn;
        canvasLayer.AddChild(turnPanel);
        world.AddElement(turnPanel);

        var unitPanel = Scenes.Instantiate<UnitPanel>();
        canvasLayer.AddChild(unitPanel);
        world.AddElement(unitPanel);

        var terrainPanel = Scenes.Instantiate<TerrainPanel>();
        canvasLayer.AddChild(terrainPanel);
        world.AddElement(terrainPanel);

        world.SpawnPlayer(0, 0, Coords.FromOffset(1, 1), "Humans", 1000);
        world.SpawnPlayer(0, 1, Coords.FromOffset(2, 2), "Humans", 1000);

        world.EndTurn();
    }

    public void Update(World world)
    {
        world.UpdateCamera();
        world.UpdateHoveredTile();
        world.UpdateDebugInfo();
        world.UpdateMapCursor();
    }

    public void Disable(World world)
    {
        world.DespawnMap();
        world.GetElement<CameraOperator>().QueueFree();
        world.RemoveElement<CameraOperator>();
        world.GetElement<Schedule>().QueueFree();
        world.RemoveElement<Schedule>();
    }
}