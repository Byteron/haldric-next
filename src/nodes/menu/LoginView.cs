using Godot;
using RelEcs;
using RelEcs.Godot;
using System;

public partial class LoginView : Control
{
    [Signal] public delegate void LoginPressed(string email, string password, string username);
    [Signal] public delegate void CancelPressed();

     LineEdit _usernameText;
     LineEdit _emailText;
     LineEdit _passwordText;

     Label _warnLabel;

    public override void _Ready()
    {
        _usernameText = GetNode<LineEdit>("PanelContainer/CenterContainer/VBoxContainer/Username/LineEdit");
        _emailText = GetNode<LineEdit>("PanelContainer/CenterContainer/VBoxContainer/Email/LineEdit");
        _passwordText = GetNode<LineEdit>("PanelContainer/CenterContainer/VBoxContainer/Password/LineEdit");

        _warnLabel = GetNode<Label>("PanelContainer/CenterContainer/VBoxContainer/WarnLabel");
    }

    void OnLoginButtonPressed()
    {
        var username = _usernameText.Text;
        var email = _emailText.Text;
        var password = _passwordText.Text;

        if (string.IsNullOrEmpty(email))
        {
            _warnLabel.Text = "no email defined!";
        }
        else if (string.IsNullOrEmpty(password))
        {
            _warnLabel.Text = "no password defined!";
        }
        else
        {
            EmitSignal(nameof(LoginPressed), email, password, username);
        }
    }

    void OnCancelButtonPressed()
    {
        EmitSignal(nameof(CancelPressed));
    }
}
