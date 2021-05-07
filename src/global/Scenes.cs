using Godot;
using System;

public partial class Scenes : Node
{
    public static Scenes Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }
}
