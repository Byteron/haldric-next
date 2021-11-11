using Bitron.Ecs;
using Godot;
using Nakama;
public partial class MatchView : Control
{
    Label _label;

    IMatchmakerTicket _ticket;
    IMatch _match;

    public override void _Ready()
    {
        _label = GetNode<Label>("PanelContainer/CenterContainer/VBoxContainer/Label");

        Network.Instance.Socket.ReceivedMatchmakerMatched += OnReceivedMatchmakerMatched;
    }

    public override void _ExitTree()
    {
        Network.Instance.Socket.ReceivedMatchmakerMatched -= OnReceivedMatchmakerMatched;
    }

    void OnReceivedMatchmakerMatched(IMatchmakerMatched matched)
    {
        var opponents = string.Join(",\n  ", matched.Users);
        _label.Text = "Matched opponents: " + opponents;
    }

    void OnCreateButtonPressed()
    {
        CreateMatch();
    }

    void OnJoinButtonPressed()
    {
        JoinMatch();
    }

    void OnCancelButtonPressed()
    {
        var socket = Network.Instance.Socket;

        if (_ticket != null)
        {
            socket.RemoveMatchmakerAsync(_ticket);
            _ticket = null;
            _label.Text = "Left Matchmaking";
        }
        else if (_match != null)
        {
            socket.LeaveMatchAsync(_match);
            _match = null;
            _label.Text = "Closed Match";
        }
        else
        {
            Main.Instance.World.GetResource<GameStateController>().PopState();
        }
    }

    private async void CreateMatch()
    {
        if (_ticket != null || _match != null)
        {
            return;
        }

        var socket = Network.Instance.Socket;
        _match = await socket.CreateMatchAsync();
        _label.Text = "Match Created: " + _match.Id;
    }

    private async void JoinMatch()
    {
        if (_ticket != null || _match != null)
        {
            return;
        }

        var socket = Network.Instance.Socket;

        var query = "*";
        var minCount = 2;
        var maxCount = 2;

        _ticket = await socket.AddMatchmakerAsync(query, minCount, maxCount);
        _label.Text = "Looking for Match...";
    }
}
