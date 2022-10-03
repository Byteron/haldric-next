using RelEcs;

public partial class TestState : GameState
{
    public override void Init()
    {
        Enter.Add(new EnterSystem());
        Exit.Add(new ExitSystem());

        Update
            .Add(new UpdateCameraSystem())
            .Add(new UpdateHoveredTileSystem())
            .Add(new UpdateMapCursorSystem())
            .Add(new TurnEndSystem())
            .Add(new UpdateDebugInfoSystem());
    }

    class EnterSystem : ISystem
    {
        public World World { get; set; }

        public void Run(World world)
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
            turnPanel.EndTurnButton.Pressed += () => world.Send(new TurnEndTrigger());
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

            world.Send(new TurnEndTrigger());
        }
    }

    class ExitSystem : ISystem
    {
        public World World { get; set; }

        public void Run(World world)
        {
            world.GetElement<CameraOperator>().QueueFree();
            world.RemoveElement<CameraOperator>();
            world.GetElement<Schedule>().QueueFree();
            world.RemoveElement<Schedule>();
        }
    }
}