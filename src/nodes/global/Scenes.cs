using Godot;
using System;

public partial class Scenes : Node
{
    public static Scenes Instance { get; private set; }

    [Export] public PackedScene EditorView;
    [Export] public PackedScene MainMenuView;
    [Export] public PackedScene LoadingStateView;
    [Export] public PackedScene DebugView;
    [Export] public PackedScene HUDView;

    [Export] public PackedScene Cursor3D;
    
    [Export] public PackedScene CameraOperator;

    [Export] public PackedScene FloatingLabel;
    [Export] public PackedScene UnitPlate;
    
    public override void _Ready()
    {
        Instance = this;
    }
}
