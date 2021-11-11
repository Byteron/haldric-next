using Godot;
using Bitron.Ecs;
using System;

public partial class LoginView : Control
{
    private LineEdit _usernameText;
    private LineEdit _emailText;
    private LineEdit _passwordText;

    private Label _warnLabel;

    public override void _Ready()
    {
        _usernameText = GetNode<LineEdit>("PanelContainer/CenterContainer/VBoxContainer/Username/LineEdit");
        _emailText = GetNode<LineEdit>("PanelContainer/CenterContainer/VBoxContainer/Email/LineEdit");
        _passwordText = GetNode<LineEdit>("PanelContainer/CenterContainer/VBoxContainer/Password/LineEdit");

        _warnLabel = GetNode<Label>("PanelContainer/CenterContainer/VBoxContainer/WarnLabel");
    }
    
    void OnLoginButtonPressed()
    {
        Login();
    }

    async void Login()
    {
        var username = _usernameText.Text;
        var email = _emailText.Text;
        var password = _passwordText.Text;
        
        if (string.IsNullOrEmpty(username))
        {
            _warnLabel.Text = "no username defined!";
        }
        else if (string.IsNullOrEmpty(email))
        {
            _warnLabel.Text = "no email defined!";
        }
        else if (string.IsNullOrEmpty(password))
        {
            _warnLabel.Text = "no password defined!";
        }
        else
        {
            var message = await Network.Instance.Login(email, password, username);

            if (string.IsNullOrEmpty(message))
            {
                _warnLabel.Text = "Logged In!";
            }
            else
            {
                _warnLabel.Text = message;
            }
        }
    }

    void OnCancelButtonPressed()
    {
        Main.Instance.World.GetResource<GameStateController>().PopState();
    }
}
