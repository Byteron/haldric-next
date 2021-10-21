using Godot;
using System.Collections.Generic;

namespace Haldric.Wdk
{
    public partial class Defenses : Node
    {
        [Export] int Flat = 0;
        [Export] int Forested = 0;
        [Export] int Rough = 0;
        [Export] int Rocky = 0;
        [Export] int Sandy = 0;
        [Export] int Aqueous = 0;
        [Export] int Cavernous = 0;
        [Export] int Settled = 0;
        [Export] int Fortified = 0;
    }
}