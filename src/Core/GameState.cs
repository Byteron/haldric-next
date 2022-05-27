using Godot;
using RelEcs;

public partial class GameState : Node3D
    {
        public SystemGroup InitSystems = new SystemGroup();
        public SystemGroup InputSystems = new SystemGroup();
        public SystemGroup UpdateSystems = new SystemGroup();
        public SystemGroup ContinueSystems = new SystemGroup();
        public SystemGroup PauseSystems = new SystemGroup();
        public SystemGroup ExitSystems = new SystemGroup();

        public virtual void Init(GameStateController gameStates) { }
    }

