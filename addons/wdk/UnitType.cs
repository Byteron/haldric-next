using Godot;
using System;

namespace Haldric.Wdk
{
    public partial class UnitType : Node3D
    {
        public Attributes Attributes;
        public Defenses Defenses;
        public UnitView UnitView;
        public Node Attacks;

        public override void _Ready()
        {
            Attributes = GetNode<Attributes>("Attributes");
            Defenses = GetNode<Defenses>("Defenses");
            UnitView = GetNode<UnitView>("UnitView");
            Attacks = GetNode<Node>("Attacks");
        }
    }
}
