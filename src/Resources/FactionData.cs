using Godot.Collections;
using Godot;

public partial class FactionData : Resource
{
    [Export] public string Name;
    [Export] public Array<string> Recruits;
    [Export] public Array<string> Leaders;
}