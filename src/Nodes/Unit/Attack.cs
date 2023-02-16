using Godot;
using System;

public partial class Attack : Node
{
    [Export] public int Damage = 0;
    [Export] public int Strikes = 0;
    [Export] public DamageType DamageType = DamageType.Slash;
    [Export] public int Range = 1;
    [Export] public PackedScene Projectile;


    public override string ToString()
    {
        return $"{Name} {Damage}x{Strikes}~{Range} ({DamageType.ToString()})";
    }
}
