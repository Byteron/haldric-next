using Godot;
using System;

public partial class MatchListing : Button
{
    private Label _nameLabel;
    private Label _playersLabel;
    
    public override void _Ready()
    {
        _nameLabel = GetNode<Label>("MarginContainer/HBoxContainer/NameLabel");    
        _playersLabel = GetNode<Label>("MarginContainer/HBoxContainer/PlayersLabel");    
    }

    public void UpdateInfo(string name, int players)
    {
        _nameLabel.Text = name;
        _playersLabel.Text = $"{players}";
    }
}
