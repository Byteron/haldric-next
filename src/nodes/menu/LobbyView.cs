using System.Collections.Generic;
using Godot;
using Nakama;

public partial class LobbyView : Control
{
    [Signal] public delegate void MatchSelected(string matchId);
    [Signal] public delegate void MessageSubmitted(string message);
    [Signal] public delegate void ScenarioSelected(string mapName);
    [Signal] public delegate void RefreshButtonPressed();
    [Signal] public delegate void JoinButtonPressed();
    [Signal] public delegate void CreateButtonPressed();
    [Signal] public delegate void QueueButtonPressed();
    [Signal] public delegate void BackButtonPressed();
    [Signal] public delegate void CancelButtonPressed();

    [Export] private PackedScene ChatMessageView;
    [Export] private PackedScene MatchListing;

    private VBoxContainer _userListContainer;
    private VBoxContainer _matchListContainer;
    private VBoxContainer _messages;
    private LineEdit _input;

    private OptionButton _scenarioOptions;
    private Button _queueButton;
    private Button _joinButton;
    private Label _infoLabel;

    public override void _Ready()
    {
        _userListContainer = GetNode<VBoxContainer>("PanelContainer/HBoxContainer/VBoxContainer2/Panel/VBoxContainer/UserList");
        _matchListContainer = GetNode<VBoxContainer>("PanelContainer/HBoxContainer/VBoxContainer/Panel2/MarginContainer/MatchList");

        _messages = GetNode<VBoxContainer>("PanelContainer/HBoxContainer/VBoxContainer/Panel/MarginContainer/Messages");
        _input = GetNode<LineEdit>("PanelContainer/HBoxContainer/VBoxContainer/HBoxContainer/LineEdit");

        _queueButton = GetNode<Button>("PanelContainer/HBoxContainer/VBoxContainer2/HBoxContainer/QueueButton");
        _joinButton = GetNode<Button>("PanelContainer/HBoxContainer/VBoxContainer2/JoinButton");
        _infoLabel = GetNode<Label>("PanelContainer/HBoxContainer/VBoxContainer2/Label");
        _scenarioOptions = GetNode<OptionButton>("PanelContainer/HBoxContainer/VBoxContainer2/MapOptionButton");

        foreach (var mapName in Data.Instance.Maps.Keys)
        {
            _scenarioOptions.AddItem(mapName);
        }

        _scenarioOptions.Select(0);
        OnMapOptionButtonItemSelected(0);
    }

    public void UpdateInfoLabel(string text)
    {
        _infoLabel.Text = text;
    }

    public void UpdateMatchList(IApiMatchList matchList)
    {
        foreach (Node child in _matchListContainer.GetChildren())
        {
            _matchListContainer.RemoveChild(child);
            child.QueueFree();
        }
        
        foreach (var match in matchList.Matches)
        {
            var listing = MatchListing.Instantiate<MatchListing>();
            listing.Connect("pressed", new Callable(this, nameof(OnMatchSelected)), new Godot.Collections.Array() { match.MatchId });
            _matchListContainer.AddChild(listing);

            listing.UpdateInfo(match.MatchId, match.Size);
        }
    }

    public void DisableQueueButton()
    {
        _queueButton.Disabled = true;
    }

    public void EnableJoinButton()
    {
        _queueButton.Disabled = false;
    }

    public void UpdateUsers(string username, List<IUserPresence> users)
    {
        foreach (Node child in _userListContainer.GetChildren())
        {
            _userListContainer.RemoveChild(child);
            child.QueueFree();
        }

        foreach (var user in users)
        {
            var label = new Label();
            label.Text = user.Username;

            if (user.Username == username)
            {
                label.Modulate = new Color("AAAAFF");
            }

            _userListContainer.AddChild(label);
        }
    }

    public void NewMessage(string username, string message, string time)
    {
        var messageView = ChatMessageView.Instantiate<ChatMessageView>();
        messageView.Message = message;
        messageView.User = username;
        messageView.Time = time;

        _messages.AddChild(messageView);

        if (_messages.GetChildCount() > 20)
        {
            var child = _messages.GetChild(0);
            _messages.RemoveChild(child);
            child.QueueFree();
        }
    }

    private void OnMatchSelected(string matchId)
    {
        EmitSignal(nameof(MatchSelected), matchId);
    }

    private void OnRefreshButtonPressed()
    {
        EmitSignal(nameof(RefreshButtonPressed));
    }
    
    private void OnJoinButtonPressed()
    {
        EmitSignal(nameof(JoinButtonPressed));
    }
    private void OnCreateButtonPressed()
    {
        EmitSignal(nameof(CreateButtonPressed));
    }

    private void OnQueueButtonPressed()
    {
        EmitSignal(nameof(QueueButtonPressed));
    }

    private void OnCancelButtonPressed()
    {
        EmitSignal(nameof(CancelButtonPressed));
    }

    private void OnMapOptionButtonItemSelected(int index)
    {
        EmitSignal(nameof(ScenarioSelected), _scenarioOptions.GetItemText(index));
    }

    private void OnLineEditTextSubmitted(string text)
    {
        OnSendButtonPressed();
    }

    private void OnSendButtonPressed()
    {
        if (!string.IsNullOrEmpty(_input.Text))
        {
            EmitSignal(nameof(MessageSubmitted), _input.Text);
            _input.Text = "";
        }
    }

    private void OnBackButtonPressed()
    {
        EmitSignal(nameof(BackButtonPressed));
    }
}
