using Godot;
using System;

namespace Haldric.Wdk
{
    public partial class Attributes : Node
    {
        [Export] public string Id = "";
        [Export] public int Health = 30;
        [Export] public int Actions = 5;
        [Export] public int Experience = 40;
    }
}
