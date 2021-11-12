using System.Threading.Tasks;
using Bitron.Ecs;
using Godot;
using Nakama;
public partial class MatchView : Control
{
    Label _label;

    ISocket _socket;
    IMatchmakerTicket _ticket;
    IMatch _match;
    IUserPresence _localPlayer;

    public override void _Ready()
    {
        _label = GetNode<Label>("PanelContainer/CenterContainer/VBoxContainer/Label");

        _socket = Network.Instance.Socket;

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
            _label.Text = "Left Matchmaking";
        }
        else if (_match != null)
        {
            _socket.LeaveMatchAsync(_match);
            _match = null;
            _label.Text = "Closed Match";
        }
        else
        {
            Main.Instance.World.GetResource<GameStateController>().PopState();
        }
    }

    private async void CreateMatchWith(IMatchmakerMatched matched)
    {
        _localPlayer = matched.Self.Presence;
        _match = await _socket.JoinMatchAsync(matched);
        _label.Text = _localPlayer.Username + " Joined Match: " + _match.Id;
    }

    private async Task CreateMatch()
    {
        if (_ticket != null || _match != null)
        {
            return;
        }

        _match = await _socket.CreateMatchAsync();
        _label.Text = "Match Created: " + _match.Id;

    }

    private async void JoinMatchmaking()
    {
        if (_ticket != null || _match != null)
        {
            return;
        }

        var query = "*";
        var minCount = 2;
        var maxCount = 2;

        _ticket = await _socket.AddMatchmakerAsync(query, minCount, maxCount);
        _label.Text = "Looking for Match...";
    }
}
