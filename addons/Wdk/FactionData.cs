using System.Collections.Generic;
using Godot;

public partial class FactionData : Resource
{
    [Export] public string Name;
    [Export] public List<string> Recruits;
    [Export] public List<string> Leaders;
}