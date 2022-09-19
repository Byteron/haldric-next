using RelEcs;
using Godot;

public partial class PlayState : GameState
{
    public override void Init()
    {
        Enter.Add(new EnterSystem());
        Exit.Add(new ExitSystem());

        Update
            .Add(new UpdateCameraSystem())
            .Add(new UpdateHoveredTileSystem())
            .Add(new UpdateMapCursorSystem())
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