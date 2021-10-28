using Godot;
using System;

public partial class Scenes : Node
{
    public static Scenes Instance { get; private set; }

    [Export] public PackedScene EditorView;
    [Export] public PackedScene MainMenuView;
    [Export] public PackedScene AttackSelectionView;
    [Export] public PackedScene RecruitSelectionView;
    [Export] public PackedScene LoadingStateView;
    [Export] public PackedScene DebugView;
    [Export] public PackedScene HUDView;

    [Export] public PackedScene TerrainHighlighter;
    
    [Export] public PackedScene CameraOperator;

    [Export] public PackedScene FloatingLabel;
    [Export] public PackedScene UnitPlate;

    [Export] public PackedScene Cursor3D;
    [Export] public PackedScene FlagView;
    
    public override void _Ready()
    {
        Instance = this;
    }
}
