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

    [Export] PackedScene FloatingLabel;
    [Export] PackedScene UnitPlate;

    [Export] PackedScene Cursor3D;
    [Export] PackedScene FlagView;

    public override void _Ready()
    {
        Instance = this;
    }

    public static CT Instantiate<CT>() where CT: Node
    {
        return (Instance.Get(typeof(CT).ToString()) as PackedScene).Instantiate<CT>();
    }
}
