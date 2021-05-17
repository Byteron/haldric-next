using Godot;
using System;

public partial class Scenes : Node
{
    public static Scenes Instance { get; private set; }

    [Export] public PackedScene EditorView;
    [Export] public PackedScene LocationHighlight;
    
    public override void _Ready()
    {
        Instance = this;
    }
}
