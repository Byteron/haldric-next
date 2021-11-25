using System.Collections.Generic;
using Godot;

namespace Haldric.Wdk
{
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
        [Export] public List<DamageType> Weaknesses;
        [Export] public List<DamageType> Calamities;
        [Export] public List<DamageType> Resistances;
        [Export] public List<DamageType> Immunities;
        [Export] public List<string> Advancements;

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
}
