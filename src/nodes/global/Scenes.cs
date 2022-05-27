using Godot;

public partial class Scenes : Node
{
    static Scenes Instance { get; set; }

    [Export] PackedScene _editorView;
    [Export] PackedScene _mainMenuView;
    [Export] PackedScene _loginView;
    [Export] PackedScene _lobbyView;
    [Export] PackedScene _attackSelectionView;
    [Export] PackedScene _factionSelectionView;
    [Export] PackedScene _scenarioSelectionView;
    [Export] PackedScene _recruitSelectionView;
    [Export] PackedScene _loadingStateView;

    [Export] PackedScene _debugPanel;

    [Export] PackedScene _sidePanel;
    [Export] PackedScene _terrainPanel;
    [Export] PackedScene _turnPanel;
    [Export] PackedScene _unitPanel;
    
    [Export] PackedScene _terrainHighlighter;

    [Export] PackedScene _cameraOperator;

    [Export] PackedScene _floatingLabel;
    [Export] PackedScene _unitPlate;

    [Export] PackedScene _cursor3D;
    [Export] PackedScene _flagView;

    public override void _Ready()
    {
        Instance = this;
    }

    public static CT Instantiate<CT>() where CT: Node {
        return (Instance.Get(typeof(CT).ToString()) as PackedScene).Instantiate<CT>();
    }
}
