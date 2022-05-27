using Godot;
using RelEcs;
using Haldric.Wdk;

public partial class EditorState : GameState
{
    public override void Init(GameStateController gameStates)
    {
        InitSystems.Add(new EditorStateInitSystem())
            .Add(new SpawnCameraOperatorSystem(this));

        InputSystems.Add(new ChangeDaytimeSystem())
            .Add(new EditorEditTerrainSystem(this))
            .Add(new EditorEditPlayerSystem(this));

        UpdateSystems.Add(new UpdateTerrainInfoSystem())
            .Add(new UpdateHoveredLocationSystem(this))
            .Add(new UpdateMapCursorSystem())
            .Add(new UpdateCameraOperatorSystem())
            .Add(new UpdateStatsInfoSystem())
            .Add(new UpdateMapTriggerSystem())
            .Add(new UpdateTerrainMeshEventSystem())
            .Add(new UpdateTerrainFeaturePopulatorEventSystem())
            .Add(new SaveMapTriggerSystem())
            .Add(new LoadMapEventSystem())
            .Add(new DespawnMapTriggerSystem())
            .Add(new SpawnScheduleTriggerSystem(this))
            .Add(new SpawnMapTriggerSystem(this))
            .Add(new ChangeDaytimeTriggerSystem());

        ExitSystems.Add(new EditorStateExitSystem())
            .Add(new DespawnCameraOperatorSystem());
    }
}

public class EditorStateExitSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.RemoveElement<Commander>();
        commands.RemoveElement<EditorView>();
        commands.RemoveElement<Schedule>();
        commands.Send(new DespawnMapTrigger());
    }
}

public class EditorStateInitSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.AddElement(new Commander());

        var editorView = Scenes.Instantiate<EditorView>();
        editorView.Commands = commands;
        commands.GetElement<CurrentGameState>().State.AddChild(editorView);

        commands.AddElement(editorView);

        commands.Send(new SpawnScheduleTrigger("DefaultSchedule"));
        commands.Send(new SpawnMapTrigger());
        commands.Send(new ChangeDaytimeTrigger());
    }
}

public class ChangeDaytimeSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<GameStateController>(out var gameStates))
        {
            return;
        }

        if (Input.IsActionPressed("ui_cancel"))
        {
            gameStates.PopState();
        }

        if (Input.IsActionPressed("ui_accept"))
        {
            commands.Send(new ChangeDaytimeTrigger());
        }
    }
}
