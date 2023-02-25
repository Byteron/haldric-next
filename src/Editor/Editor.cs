using Godot;

namespace Haldric;

public partial class Editor : Node3D
{
    Map _map = default!;

    [Export] PackedScene _mapScene = default!;
    [Export] CameraOperator _cameraOperator = default!;

    public override void _Input(InputEvent e)
    {
        if (e.IsActionPressed("ui_cancel"))
        {
            GetTree().ChangeSceneToFile("res://src/Menu/MainMenu.tscn");
        }
    }

    public override void _Ready()
    {
        _map = _mapScene.Instantiate<Map>();
        AddChild(_map);

        _map.Initialize(40, 40);
    }
}