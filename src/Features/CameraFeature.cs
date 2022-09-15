using RelEcs;
using Godot;

public class CameraFeature : Feature
{
    public override void Init()
    {
        InitSystems.Add(new InitCameraFeatureSystem());
        EnableSystems.Add(new EnableCameraFeatureSystem());
        DisableSystems.Add(new DisableCameraFeatureSystem());
        UpdateSystems.Add(new UpdateCameraSystem());
    }
}

public class InitCameraFeatureSystem : ISystem
{
    public World World { get; set; }

    public void Run()
    {
        var tree = this.GetTree();
        var camera = Scenes.Instantiate<CameraOperator>();
        this.AddElement(camera);
        tree.CurrentScene.AddChild(camera);
        camera.Camera.Current = false;
    }
}

public class EnableCameraFeatureSystem : ISystem
{
    public World World { get; set; }

    public void Run()
    {
        var camera = this.GetElement<CameraOperator>();
        camera.Camera.Current = true;
    }
}

public class DisableCameraFeatureSystem : ISystem
{
    public World World { get; set; }

    public void Run()
    {
        var camera = this.GetElement<CameraOperator>();
        camera.Camera.Current = false;
    }
}