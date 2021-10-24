using System.Collections.Generic;
using Godot;

namespace Haldric.Wdk
{
    public partial class UnitType : Node3D
    {
        [Export] public string Id = "";
        [Export] public int Health = 30;
        [Export] public int Moves = 5;
        [Export] public int Experience = 40;

        [Export] public List<DamageType> Weaknesses;
        [Export] public List<DamageType> Calamities;
        [Export] public List<DamageType> Resistances;
        [Export] public List<DamageType> Immunities;

        public UnitView UnitView;
        public Node Attacks;

        public override void _Ready()
        {
            UnitView = GetNode<UnitView>("UnitView");
            Attacks = GetNode<Node>("Attacks");
        }
    }
}
