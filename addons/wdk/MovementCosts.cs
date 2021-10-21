using Godot;
using System.Collections.Generic;

namespace Haldric.Wdk
{
    public partial class MovementCosts : Node
    {
        [Export] int Flat = 1;
        [Export] int Forested = 1;
        [Export] int Rough = 1;
        [Export] int Rocky = 1;
        [Export] int Sandy = 1;
        [Export] int Aqueous = 1;
        [Export] int Cavernous = 1;
        [Export] int Settled = 1;
        [Export] int Fortified = 1;
    }
}