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

        public void Run()
        {
            var scene = this.GetTree().CurrentScene;
            var data = this.GetElement<ScenarioData>();

            var camera = Scenes.Instantiate<CameraOperator>();
            this.AddElement(camera);
            scene.AddChild(camera);

            var mapData = data.Maps["Valley"];
            this.SpawnSchedule("DefaultSchedule", 1);
            this.SpawnMap(mapData);
            this.UpdateTerrainMesh();
            this.UpdateTerrainProps();


            this.AddElement(new LocalPlayer { Id = 0 });
            this.AddElement(new Scenario());

            var canvas = this.GetElement<Canvas>();
            var canvasLayer = canvas.GetCanvasLayer(1);

            var turnPanel = Scenes.Instantiate<TurnPanel>();
            turnPanel.EndTurnButton.Pressed += () => this.Send(new TurnEndTrigger());
            canvasLayer.AddChild(turnPanel);
            this.AddElement(turnPanel);

            var unitPanel = Scenes.Instantiate<UnitPanel>();
            canvasLayer.AddChild(unitPanel);
            this.AddElement(unitPanel);

            var terrainPanel = Scenes.Instantiate<TerrainPanel>();
            canvasLayer.AddChild(terrainPanel);
            this.AddElement(terrainPanel);

            this.SpawnPlayer(0, 0, Coords.FromOffset(1, 1), "Humans", 1000);
            this.SpawnPlayer(0, 1, Coords.FromOffset(2, 2), "Humans", 1000);

            this.Send(new TurnEndTrigger());
        }
    }

    class ExitSystem : ISystem
    {
        public World World { get; set; }

        public void Run()
        {
            this.GetElement<CameraOperator>().QueueFree();
            this.RemoveElement<CameraOperator>();
            this.GetElement<Schedule>().QueueFree();
            this.RemoveElement<Schedule>();
        }
    }
}