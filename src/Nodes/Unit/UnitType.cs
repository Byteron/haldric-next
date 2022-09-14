using Godot.Collections;
using Godot;

public partial class UnitType : Node3D
{
    [Export] public string Id = "";
    [Export] public int Cost = 15;
    [Export] public int Level = 1;
    [Export] public int Health = 30;
    [Export] public int Actions = 1;
    [Export] public int Moves = 5;
    [Export] public int Experience = 40;

    [Export] public Alignment Alignment;
    [Export] public Array<DamageType> Weaknesses;
    [Export] public Array<DamageType> Calamities;
    [Export] public Array<DamageType> Resistances;
    [Export] public Array<DamageType> Immunities;
    [Export] public Array<string> Advancements;

    public UnitView UnitView;
    public Node Traits;
    public Node Attacks;

    public override void _Ready()
    {
        UnitView = GetNode<UnitView>("UnitView");
        Traits = GetNode<Node>("Traits");
        Attacks = GetNode<Node>("Attacks");
    }
}