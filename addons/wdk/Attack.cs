using Godot;
using System;

namespace Haldric.Wdk
{
    public partial class Attack : Node
    {
        [Export] public int Costs = 0;
        [Export] public int Damage = 0;
        [Export] public int Strikes = 0;
        [Export] public DamageType DamageType = DamageType.Slash;
        [Export] public int Range = 1;
    }
}
