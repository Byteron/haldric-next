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

        [Export] public NodePath AnimationPlayerNode;

        private AnimationPlayer _animationPlayer;

        public UnitView UnitView;
        public Node Attacks;

        public override void _Ready()
        {
            UnitView = GetNode<UnitView>("UnitView");
            Attacks = GetNode<Node>("Attacks");

            if (AnimationPlayerNode != null)
            {
                _animationPlayer = GetNode<AnimationPlayer>(AnimationPlayerNode);
            }
        }

        public void Play(string animName)
        {
            if (_animationPlayer == null)
            {
                return;
            }

            else if (!_animationPlayer.HasAnimation(animName))
            {
                return;
            }

            _animationPlayer.Play(animName);
        }
    }
}
