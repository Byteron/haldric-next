using Godot;
using System;

public partial class MainMenu : Control
{
    void OnPlayButtonPressed()
    {
        GD.Print("Play Button Pressed");
    }

    void OnTestButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://src/Scenario/Scenario.tscn");
    }

    void OnEditorButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://src/Editor/Editor.tscn");
    }

    void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}