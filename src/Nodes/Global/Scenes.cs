using Godot;

public partial class Scenes : Node
{
    static Scenes Instance { get; set; }

    [Export] PackedScene EditorView;
    [Export] PackedScene MainMenuView;
    [Export] PackedScene LoginView;
    [Export] PackedScene LobbyView;
    [Export] PackedScene AttackSelectionView;
    [Export] PackedScene FactionSelectionView;
    [Export] PackedScene ScenarioSelectionView;
    [Export] PackedScene RecruitSelectionView;
    [Export] PackedScene LoadingStateView;

    [Export] PackedScene DebugPanel;

    [Export] PackedScene SidePanel;
    [Export] PackedScene TerrainPanel;
    [Export] PackedScene TurnPanel;
    [Export] PackedScene UnitPanel;
    
    [Export] PackedScene TerrainHighlighter;

    [Export] PackedScene CameraOperator;
    [Export] PackedScene WorldEnvironment;
    
    [Export] PackedScene FloatingLabel;
    [Export] PackedScene UnitPlate;

    [Export] PackedScene Cursor3D;
    [Export] PackedScene FlagView;

    public override void _Ready()
    {
        Instance = this;
    }

    public static T Instantiate<T>() where T: Node
    {
        return (Instance.Get(typeof(T).Name).AsGodotObject() as PackedScene).Instantiate<T>();
    }
}
