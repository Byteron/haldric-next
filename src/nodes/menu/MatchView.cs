using System.Threading.Tasks;
using Bitron.Ecs;
using Godot;
using Nakama;
public partial class MatchView : Control
{
    public string MapName { get; set; } = "";
    public int PlayerCount { get; set; } = 2;

    Label _infoLabel;
    Label _mapLabel;

    ISocket _socket;
    IMatchmakerTicket _ticket;

    IMatch _match;

    public override void _Ready()
    {
        _infoLabel = GetNode<Label>("PanelContainer/CenterContainer/VBoxContainer/InfoLabel");
        _mapLabel = GetNode<Label>("PanelContainer/CenterContainer/VBoxContainer/MapLabel");

        _mapLabel.Text = $"Map: {MapName}, Players: {PlayerCount}";

        _socket = Main.Instance.World.GetResource<ISocket>();

        _socket.ReceivedMatchmakerMatched += OnReceivedMatchmakerMatched;
    }

    public override void _ExitTree()
    {
        _socket.ReceivedMatchmakerMatched -= OnReceivedMatchmakerMatched;
    }

    void OnReceivedMatchmakerMatched(IMatchmakerMatched matched)
    {
        CreateMatchWith(matched);
    }

    void OnCreateButtonPressed()
    {
        CreateMatch();
    }

    void OnJoinButtonPressed()
    {
        JoinMatchmaking();
    }

    void OnCancelButtonPressed()
    {
        if (_ticket != null)
        {
            _socket.RemoveMatchmakerAsync(_ticket);
            _ticket = null;
            _infoLabel.Text = "Left Matchmaking";
        }
        else if (_match != null)
        {
            _socket.LeaveMatchAsync(_match);
            _match = null;
            _infoLabel.Text = "Closed Match";
        }
        else
        {
            Main.Instance.World.GetResource<GameStateController>().PopState();
        }
    }

    private async void CreateMatchWith(IMatchmakerMatched matched)
    {
        
        _match = await _socket.JoinMatchAsync(matched);
        
        var localPlayer = new LocalPlayer();

        localPlayer.Presence = matched.Self.Presence;

        var playerId = 0;
        foreach (var presence in matched.Users)
        {
            if (matched.Self.Presence.UserId == presence.Presence.UserId)
            {
                localPlayer.Side = playerId;
            }
            playerId += 1;
        }

        Main.Instance.World.AddResource(localPlayer);
        Main.Instance.World.AddResource(_match);

        Main.Instance.World.GetResource<GameStateController>().PushState(new FactionSelectionState(Main.Instance.World, MapName));
    }

    private async Task CreateMatch()
    {
        if (_ticket != null || _match != null)
        {
            return;
        }

        _match = await _socket.CreateMatchAsync();
        _infoLabel.Text = "Match Created: " + _match.Id;

    }

    private async void JoinMatchmaking()
    {
        if (_ticket != null || _match != null)
        {
            return;
        }

        var query = "*";
        var minCount = PlayerCount;
        var maxCount = PlayerCount;

        _ticket = await _socket.AddMatchmakerAsync(query, minCount, maxCount);
        _infoLabel.Text = "Looking for Match...";
    }
}
