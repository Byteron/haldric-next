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
            .Add(new UpdateMapEventSystem())
            .Add(new UpdateTerrainMeshEventSystem())
            .Add(new UpdateTerrainFeaturePopulatorEventSystem())
            .Add(new SaveMapEventSystem())
            .Add(new LoadMapEventSystem())
            .Add(new DespawnMapEventSystem())
            .Add(new SpawnScheduleEventSystem(this))
            .Add(new SpawnMapEventSystem(this))
            .Add(new ChangeDaytimeEventSystem());

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
        commands.Send(new DespawnMapEvent());
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

        commands.Send(new SpawnScheduleEvent("DefaultSchedule"));
        commands.Send(new SpawnMapEvent(40, 40));
        commands.Send(new ChangeDaytimeEvent());
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
            commands.Send(new ChangeDaytimeEvent());
        }
    }
}
