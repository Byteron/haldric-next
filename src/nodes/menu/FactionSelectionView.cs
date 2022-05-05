using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class FactionSelectionView : PanelContainer
{
    [Signal] public delegate void FactionChanged(int side, int index);
    [Signal] public delegate void PlayerChanged(int side, int index);
    [Signal] public delegate void GoldChanged(int side, int value);
    [Signal] public delegate void ContinueButtonPressed();
    [Signal] public delegate void BackButtonPressed();

    [Export]  PackedScene _playerOption;

    public int LocalPlayerId { get; set; }
    public List<string> Players { get; set; }
    public string MapName { get; set; }

    Dictionary<int, PlayerOption> _options = new Dictionary<int, PlayerOption>();

    VBoxContainer _container = null;
    Button _continueButton = null;
    Button _backButton = null;

    public override void _Ready()
    {
        _container = GetNode<VBoxContainer>("CenterContainer/VBoxContainer/VBoxContainer");
        _continueButton = GetNode<Button>("CenterContainer/VBoxContainer/HBoxContainer/ContinueButton");
        _backButton = GetNode<Button>("CenterContainer/VBoxContainer/HBoxContainer/BackButton");

        for (int side = 0; side < Players.Count; side++)
        {
            var option = _playerOption.Instantiate<PlayerOption>();
            option.Connect(nameof(PlayerOption.FactionChanged), new Callable(this, nameof(OnFactionChanged)));
            option.Connect(nameof(PlayerOption.PlayerChanged), new Callable(this, nameof(OnPlayerChanged)));
            option.Connect(nameof(PlayerOption.GoldChanged), new Callable(this, nameof(OnGoldChanged)));
            _container.AddChild(option);
            option.UpdateInfo(LocalPlayerId, side, 100, Data.Instance.Factions.Keys.ToList(), Players);
            _options.Add(side, option);
        }
    }

    public void ChangeFaction(int side, int index)
    {
        _options[side].ChangeFaction(index);
    }

    public void ChangePlayer(int side, int index)
    {
        _options[side].ChangePlayer(index);
    }

    public void ChangeGold(int side, int value)
    {
        _options[side].ChangeGold(value);
    }

    public Dictionary<int, string> GetFactions()
    {
        var factions = new Dictionary<int, string>();

        foreach (PlayerOption option in _container.GetChildren())
        {
            factions.Add(option.Side, option.Faction);
        }

        return factions;
    }

    public Dictionary<int, int> GetPlayers()
    {
        var players = new Dictionary<int, int>();

        foreach (PlayerOption option in _container.GetChildren())
        {
            players.Add(option.Side, option.PlayerId);
        }

        return players;
    }

    public Dictionary<int, int> GetPlayerGolds()
    {
        var golds = new Dictionary<int, int>();

        foreach (PlayerOption option in _container.GetChildren())
        {
            golds.Add(option.Side, option.Gold);
        }

        return golds;
    }

     void OnFactionChanged(int side, int index)
    {
        EmitSignal(nameof(FactionChanged), side, index);
    }

     void OnPlayerChanged(int side, int index)
    {
        EmitSignal(nameof(PlayerChanged), side, index);
    }

     void OnGoldChanged(int side, int value)
    {
        EmitSignal(nameof(GoldChanged), side, value);
    }

     void OnContinueButtonPressed()
    {
        _continueButton.Disabled = true;
        _backButton.Disabled = true;
        EmitSignal(nameof(ContinueButtonPressed));
    }

     void OnBackButtonPressed()
    {
        EmitSignal(nameof(BackButtonPressed));
    }
}
