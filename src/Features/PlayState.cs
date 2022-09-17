using RelEcs;
using Godot;

public partial class PlayState : GameState
{
    public override void Init()
    {
        Enter.Add(new EnterSystem());
        Exit.Add(new ExitSystem());

        Update.Add(new UpdateCameraSystem());
    }

    class EnterSystem : ISystem
    {
        public World World { get; set; }

        public void Run()
        {
            var tree = this.GetTree();

            var camera = Scenes.Instantiate<CameraOperator>();
            this.AddElement(camera);
            tree.CurrentScene.AddChild(camera);

            var env = Scenes.Instantiate<WorldEnvironment>();
            this.AddElement(env);
            tree.CurrentScene.AddChild(env);

            var mapData = this.LoadMapData("Valley");
            this.SpawnMap(mapData);
            this.UpdateTerrainMesh();
            this.UpdateTerrainProps();
        }
    }

    public class ExitSystem : ISystem
    {
        public World World { get; set; }

        public void Run()
        {
            this.GetElement<CameraOperator>().QueueFree();
            this.GetElement<WorldEnvironment>().QueueFree();
        }
    }
}