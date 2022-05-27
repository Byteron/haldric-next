using Godot;

public partial class LoadingStateView : Control
{
    public Label Label { get; set; }

    public override void _Ready()
    {
        Label = GetNode<Label>("ColorRect/CenterContainer/VBoxContainer/Label");
    }
}