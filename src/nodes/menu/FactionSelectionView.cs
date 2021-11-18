using Godot;
using Godot.Collections;

public partial class FactionSelectionView : PanelContainer
{
    [Signal] public delegate void FactionSelected(int side, int index);
    [Signal] public delegate void ContinueButtonPressed();
    [Signal] public delegate void BackButtonPressed();

    [Export] PackedScene PlayerOption;

    public int LocalPlayerSide { get; set; }
    public int PlayerCount { get; set; }
    public string MapName { get; set; }

    private Dictionary<int, string> _factions = new Dictionary<int, string>();
    Dictionary<int, PlayerOption> _options = new Dictionary<int, PlayerOption>();

    VBoxContainer _container = null;
    Button _continueButton = null;
    Button _backButton = null;

    public override void _Ready()
    {
        _container = GetNode<VBoxContainer>("CenterContainer/VBoxContainer/VBoxContainer");
        _continueButton = GetNode<Button>("CenterContainer/VBoxContainer/HBoxContainer/ContinueButton");
        _backButton = GetNode<Button>("CenterContainer/VBoxContainer/HBoxContainer/BackButton");

        for (int i = 0; i < PlayerCount; i++)
        {
            var option = PlayerOption.Instantiate<PlayerOption>();
            option.Connect("FactionSelected", new Callable(this, nameof(OnFactionSelected)));
            option.Side = i;
            option.LocalPlayerId = LocalPlayerSide;
            _container.AddChild(option);
            _options.Add(i, option);
        }
    }

    public void Select(int side, int index)
    {
        _options[side].Select(index);
    }

    public Dictionary<int, string> GetFactions()
    {
        _factions.Clear();

        foreach (PlayerOption option in _container.GetChildren())
        {
            _factions.Add(option.Side, option.Faction);
        }

        return _factions;
    }

    private void OnFactionSelected(int side, int index)
    {
        EmitSignal(nameof(FactionSelected), side, index);
    }

    private void OnContinueButtonPressed()
    {
        _continueButton.Disabled = true;
        _backButton.Disabled = true;
        EmitSignal(nameof(ContinueButtonPressed));
    }

    private void OnBackButtonPressed()
    {
        EmitSignal(nameof(BackButtonPressed));
    }
}
