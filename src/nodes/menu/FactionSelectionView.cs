using Godot;
using Godot.Collections;
using Bitron.Ecs;
using Nakama;
using Nakama.TinyJson;

public partial class FactionSelectionView : PanelContainer
{
    [Signal] public delegate void FactionSelected(int side, int index);
    [Signal] public delegate void ContinueButtonPressed(Dictionary<int, string> factions);
    [Signal] public delegate void BackButtonPressed();

    [Export] PackedScene PlayerOption;

    public int LocalPlayerSide { get; set; }
    public int PlayerCount { get; set; }
    public string MapName { get; set; }

    private Dictionary<int, string> _factions = new Dictionary<int, string>();
    Dictionary<int, PlayerOption> _options = new Dictionary<int, PlayerOption>();

    VBoxContainer _container;

    public override void _Ready()
    {
        _container = GetNode<VBoxContainer>("CenterContainer/VBoxContainer/VBoxContainer");

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

    private void OnFactionSelected(int side, int index)
    {
        EmitSignal(nameof(FactionSelected), side, index);
    }

    private void OnContinueButtonPressed()
    {
        foreach (PlayerOption option in _container.GetChildren())
        {
            _factions.Add(option.Side, option.Faction);
        }

        EmitSignal(nameof(ContinueButtonPressed), _factions);
    }

    private void OnBackButtonPressed()
    {
        EmitSignal(nameof(BackButtonPressed));
    }
}
