using System.Collections.Generic;
using Godot;

public partial class PlayerOption : HBoxContainer
{
    [Signal] public delegate void FactionChanged(int side, int index);
    [Signal] public delegate void PlayerChanged(int side, int index);
    [Signal] public delegate void GoldChanged(int side, int value);

    public int LocalPlayerId { get; set; }
    public int Side { get; set; }
    public int Gold { get; set; }
    public int PlayerId { get => _playerOptions.GetSelectedId(); }
    public string Faction { get => _factionOptions.GetItemText(_factionOptions.GetSelectedId()); }

    Label _sideLabel;
    Label _goldLabel;
    HSlider _goldSlider;
    OptionButton _playerOptions;
    OptionButton _factionOptions;

    public override void _Ready()
    {
        _sideLabel = GetNode<Label>("SideLabel");
        _goldLabel = GetNode<Label>("GoldLabel");

        _goldSlider = GetNode<HSlider>("GoldSlider");

        _playerOptions = GetNode<OptionButton>("PlayerOptionButton");
        _factionOptions = GetNode<OptionButton>("FactionOptionButton");
    }

    public void UpdateInfo(int localPlayerId, int side, int gold, List<string> factions, List<string> players)
    {
        LocalPlayerId = localPlayerId;
        Side = side;
        Gold = gold;
        
        if (localPlayerId != 0)
        {
            _playerOptions.Disabled = true;
            _goldSlider.Hide();
        }
        
        if (localPlayerId != Side)
        {
            _factionOptions.Disabled = true;
        }

        foreach (var faction in factions)
        {
            _factionOptions.AddItem(faction);
        }

        foreach (var player in players)
        {
            _playerOptions.AddItem(player);
        }

        if (_factionOptions.ItemCount > 0)
        {
            _factionOptions.Select(0);
        }

        if (_playerOptions.ItemCount > 0)
        {
            _playerOptions.Select(side);
        }

        ChangeGold(Gold);

        _sideLabel.Text = $"Side: {side}";
    }

    public void UpdatePlayers(List<string> players)
    {
        _playerOptions.Clear();
        
        foreach (var player in players)
        {
            _playerOptions.AddItem(player);
        }
    }

    public void ChangePlayer(int index)
    {
        _playerOptions.Select(index);
    }

    public void ChangeGold(int value)
    {
        Gold = value;
        _goldLabel.Text = $"Gold: {Gold}";
    }

    public void ChangeFaction(int index)
    {
        _factionOptions.Select(index);
    }

    private void OnPlayerOptionButtonItemSelected(int index)
    {
        EmitSignal(nameof(PlayerChanged), Side, index);
    }

    private void OnFactionOptionButtonItemSelected(int index)
    {
        EmitSignal(nameof(FactionChanged), Side, index);
    }

    private void OnGoldSliderValueChanged(int value)
    {
        Gold = value;
        _goldLabel.Text = $"Gold: {Gold}";
        EmitSignal(nameof(GoldChanged), Side, Gold);
    }
}
