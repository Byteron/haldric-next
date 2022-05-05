using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;
using Godot;
using Haldric.Wdk;
using Nakama;
using Nakama.TinyJson;

public partial class PlayState : GameState
{
     partial class MarshallableState : Resource
    {
        public IMatchState State { get; set; }
    }

    public string MapName;
    public Dictionary<int, string> Factions;
    public Dictionary<int, int> Players;
    public Dictionary<int, int> PlayerGolds;

    public ISocket Socket;
    public IMatch Match;

    public override void Init(GameStateController gameStates)
    {
        var initSystem = new PlayStateInitSystem();
        initSystem.MapName = MapName;
        initSystem.Factions = Factions;
        initSystem.Players = Players;
        initSystem.PlayerGolds = PlayerGolds;

        InitSystems.Add(initSystem)
            .Add(new SpawnCameraOperatorSystem(this));

        InputSystems.Add(new ExitFromStateSystem())
            .Add(new SelectUnitSystem())
            .Add(new SelectTargetSystem())
            .Add(new DeselectUnitSystem())
            .Add(new RecruitInputSystem())
            .Add(new NextUnitInputSystem())
            .Add(new SuspendUnitInputSystem());

        UpdateSystems.Add(new UpdateTerrainInfoSystem())
            .Add(new UpdatePlayerInfoSystem())
            .Add(new UpdateUnitPlateSystem())
            .Add(new UpdateStatsInfoSystem())
            .Add(new UpdateHoveredLocationSystem(this))
            .Add(new PreviewPathSystem())
            .Add(new UpdateMapCursorSystem())
            .Add(new UpdateCameraOperatorSystem())
            .Add(new UpdateHoveredUnitSystem())
            .Add(new MoveUnitSystem())
            .Add(new FocusCameraEventSystem())
            .Add(new UpdateMapEventSystem())
            .Add(new UpdateTerrainMeshEventSystem())
            .Add(new UpdateTerrainFeaturePopulatorEventSystem())
            .Add(new LoadMapEventSystem())
            .Add(new DespawnMapEventSystem())
            .Add(new SpawnScheduleEventSystem(this))
            .Add(new SpawnMapEventSystem(this))
            .Add(new SpawnPlayersEventSystem())
            .Add(new SpawnPlayerEventSystem())
            .Add(new SpawnUnitEventSystem(this))
            .Add(new RecruitUnitEventSystem(this))
            .Add(new UnitHoveredEventSystem())
            .Add(new UnitDeselectedEventSystem())
            .Add(new UnitSelectedEventSystem())
            .Add(new MoveUnitEventSystem())
            .Add(new HighlightLocationsEventSystem())
            .Add(new DamageEventSystem())
            .Add(new MissEventSystem())
            .Add(new GainExperienceEventSystem())
            .Add(new AdvanceEventSystem())
            .Add(new DeathEventSystem())
            .Add(new CaptureVillageEventSystem(this))
            .Add(new SpawnFloatingLabelEventSystem())
            .Add(new TurnEndEventSystem())
            .Add(new ChangeDaytimeEventSystem())
            .Add(new CheckVictoryConditionEventSystem());

        ExitSystems.Add(new PlayStateExitSystem())
            .Add(new DespawnCameraOperatorSystem());
    }  
}

public class ExitFromStateSystem : ISystem
{
    public void Run(Commands commands)
    {
        var gameStates = commands.GetElement<GameStateController>();

        if (Input.IsActionPressed("ui_cancel"))
        {
            gameStates.PopState();
        }
    }
}

public class PlayStateExitSystem : ISystem
{
    public void Run(Commands commands)
    {
        var socket = commands.GetElement<ISocket>();
        var match = commands.GetElement<IMatch>();

        // socket.ReceivedMatchState -= OnReceivedMatchState;
        socket.LeaveMatchAsync(match);

        commands.RemoveElement<IMatch>();
        commands.RemoveElement<LocalPlayer>();
        commands.RemoveElement<MatchPlayers>();
        
        commands.RemoveElement<Commander>();
        commands.RemoveElement<Scenario>();
        commands.RemoveElement<Schedule>();

        var sidePanel = commands.GetElement<SidePanel>();
        sidePanel.QueueFree();
        commands.RemoveElement<SidePanel>();

        var terrainPanel = commands.GetElement<TerrainPanel>();
        terrainPanel.QueueFree();
        commands.RemoveElement<TerrainPanel>();

        var unitPanel = commands.GetElement<UnitPanel>();
        unitPanel.QueueFree();
        commands.RemoveElement<UnitPanel>();

        var turnPanel = commands.GetElement<TurnPanel>();
        turnPanel.QueueFree();
        commands.RemoveElement<TurnPanel>();

        commands.Send(new DespawnMapEvent());
    }
}
public partial class PlayStateInitSystem : Resource, ISystem
{
    public string MapName;
    public Dictionary<int, string> Factions;
    public Dictionary<int, int> Players;
    public Dictionary<int, int> PlayerGolds;

    ISocket socket;
    IMatch match;

    Commands commands;

    public void Run(Commands commands)
    {
        this.commands = commands;

        socket = commands.GetElement<ISocket>();
        socket.ReceivedMatchState += OnReceivedMatchState;

        match = commands.GetElement<IMatch>();

        var matchPlayers = commands.GetElement<MatchPlayers>();

        commands.AddElement(new Commander());
        commands.AddElement(new Scenario());

        var canvas = commands.GetElement<Canvas>();
        var canvasLayer = canvas.GetCanvasLayer(1);

        var turnPanel = Scenes.Instantiate<TurnPanel>();
        turnPanel.Connect(nameof(TurnPanel.EndTurnButtonPressed), new Callable(this, nameof(OnTurnEndButtonPressed)));
        canvasLayer.AddChild(turnPanel);
        commands.AddElement(turnPanel);

        var sidePanel = Scenes.Instantiate<SidePanel>();
        canvasLayer.AddChild(sidePanel);
        commands.AddElement(sidePanel);

        var unitPanel = Scenes.Instantiate<UnitPanel>();
        canvasLayer.AddChild(unitPanel);
        commands.AddElement(unitPanel);

        var terrainPanel = Scenes.Instantiate<TerrainPanel>();
        canvasLayer.AddChild(terrainPanel);
        commands.AddElement(terrainPanel);

        commands.Send(new SpawnScheduleEvent("DefaultSchedule"));
        commands.Send(new LoadMapEvent(MapName));
        commands.Send(new SpawnPlayersEvent(Factions, Players, PlayerGolds));
        commands.Send(new TurnEndEvent());
    }

    public void OnTurnEndButtonPressed()
    {
        var opCode = (int)NetworkOperation.TurnEnd;
        var state = new TurnEndMessage();
        socket.SendMatchStateAsync(match.Id, opCode, state.ToJson());
        commands.Send(new TurnEndEvent());
    }

    void OnReceivedMatchState(IMatchState state)
    {
        GD.Print("Reseived Match State");
        CallDeferred(nameof(ProcessMatchStateChange), new Marshallable<IMatchState>(state));
    }

    void ProcessMatchStateChange(Marshallable<IMatchState> marshallableState)
    {
        var state = marshallableState.Value;
        var gameStateController = commands.GetElement<GameStateController>();

        var enc = System.Text.Encoding.UTF8;
        var data = (string)enc.GetString(state.State);
        var operation = (NetworkOperation)state.OpCode;

        GD.Print($"Network Operation: {operation.ToString()}\nJSON: {data}");

        switch (operation)
        {
            case NetworkOperation.TurnEnd:
                {
                    commands.Send(new TurnEndEvent());
                    break;
                }
            case NetworkOperation.MoveUnit:
                {
                    var message = JsonParser.FromJson<MoveUnitMessage>(data);
                    commands.Send(new MoveUnitEvent() { From = message.From.Cube(), To = message.To.Cube() });
                    break;
                }
            case NetworkOperation.RecruitUnit:
                {
                    var map = commands.GetElement<Map>();
                    var message = JsonParser.FromJson<RecruitUnitMessage>(data);
                    var unitType = Data.Instance.Units[message.UnitTypeId].Instantiate<UnitType>();
                    var locEntity = map.Locations.Get(message.Coords.Cube());
                    commands.Send(new RecruitUnitEvent(message.Side, unitType, locEntity));
                    break;
                }
            case NetworkOperation.AttackUnit:
                {
                    var map = commands.GetElement<Map>();
                    var commander = commands.GetElement<Commander>();

                    var message = JsonParser.FromJson<AttackUnitMessage>(data);

                    var attackerLocEntity = map.Locations.Get(message.From.Cube());
                    var defenderLocEntity = map.Locations.Get(message.To.Cube());

                    var attackerEntity = attackerLocEntity.Get<HasUnit>().Entity;
                    var defenderEntity = defenderLocEntity.Get<HasUnit>().Entity;

                    var attackerAttacks = attackerEntity.Get<Attacks>();
                    var defenderAttacks = defenderEntity.Get<Attacks>();

                    var attackerAttackEntity = attackerAttacks.GetAttack(message.AttackerAttackId);
                    var defenderAttackEntity = defenderAttacks.GetAttack(message.DefenderAttackId);

                    var command = new CombatCommand(message.Seed, attackerLocEntity, attackerAttackEntity, defenderLocEntity, defenderAttackEntity, message.Distance);
                    commander.Enqueue(command);

                    gameStateController.PushState(new CommanderState());
                    break;
                }
        }
    }
}
