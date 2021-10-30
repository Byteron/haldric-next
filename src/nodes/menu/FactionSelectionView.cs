using Godot;
using System.Collections.Generic;
using Bitron.Ecs;

public partial class FactionSelectionView : PanelContainer
{
    [Export] PackedScene PlayerOption;

    public int PlayerCount { get; set; }
    public string MapName { get; set; }

    private Dictionary<int, string> _factions = new Dictionary<int, string>();

    VBoxContainer _container;

    public override void _Ready()
    {
        _container = GetNode<VBoxContainer>("CenterContainer/VBoxContainer/VBoxContainer");

        for (int i = 0; i < PlayerCount; i++)
        {
            var option = PlayerOption.Instantiate<PlayerOption>();
            option.Side = i;
            _container.AddChild(option);
        }
    }

    private void OnContinueButtonPressed()
    {
        foreach (PlayerOption option in _container.GetChildren())
        {
            _factions.Add(option.Side, option.Faction);
        }
        
        var gameStateController = Main.Instance.World.GetResource<GameStateController>();
        
        var playState = new PlayState(Main.Instance.World, MapName, _factions);
        gameStateController.PopState();

        gameStateController.PushState(playState);
    }

    private void OnBackButtonPressed()
    {
        Main.Instance.World.GetResource<GameStateController>().PopState();
    }
}
