using Godot;
using System.Collections.Generic;
using Bitron.Ecs;
using Nakama;
using Nakama.TinyJson;

public partial class FactionSelectionView : PanelContainer
{
    [Export] PackedScene PlayerOption;

    public int PlayerCount { get; set; }
    public string MapName { get; set; }

    private Dictionary<int, string> _factions = new Dictionary<int, string>();

    VBoxContainer _container;
    Dictionary<int, PlayerOption> _options = new Dictionary<int, PlayerOption>();

    public override void _Ready()
    {
        _container = GetNode<VBoxContainer>("CenterContainer/VBoxContainer/VBoxContainer");
        
        for (int i = 0; i < PlayerCount; i++)
        {
            var option = PlayerOption.Instantiate<PlayerOption>();
            option.Connect("FactionSelected", new Callable(this, nameof(OnFactionSelected)));
            option.Side = i;
            _container.AddChild(option);
            _options.Add(i, option);
        }

        Network.Instance.Socket.ReceivedMatchState += OnMatchStateReceived;
    }

    private void OnMatchStateReceived(IMatchState state)
    {
        var enc = System.Text.Encoding.UTF8;
        var operation = (NetworkOperation)state.OpCode;
        var data = (string)enc.GetString(state.State);

        switch (operation)
        {
            case NetworkOperation.FactionSelected:
                var dict = JsonParser.FromJson<Dictionary<string, int>>(data);
                _options[dict["side"]].Select(dict["index"]);
                break;
        }
    }

    private void OnFactionSelected(int side, int index)
    {
        var matchId = Network.Instance.Match.Id;
        var opCode = (int)NetworkOperation.FactionSelected;
        
        var newState = new Dictionary<string, int>
        { 
            { "side", side },
            { "index", index },
        };

        Network.Instance.Socket.SendMatchStateAsync(matchId, opCode, newState.ToJson());
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
