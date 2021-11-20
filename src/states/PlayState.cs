using Bitron.Ecs;
using Godot;
using Godot.Collections;
using Haldric.Wdk;
using Nakama;
using Nakama.TinyJson;

public partial class PlayState : GameState
{
    private partial class MarshallableState : Resource
    {
        public IMatchState State { get; set; }
    }

    private string _mapName;
    private Dictionary<int, string> _factions;
    private ISocket _socket;
    private IMatch _match;

    public PlayState(EcsWorld world, string mapName, Dictionary<int, string> factions) : base(world)
    {
        _mapName = mapName;
        _factions = factions;

        AddInitSystem(new SpawnCameraOperatorSystem(this));

        AddInputSystem(new SelectUnitSystem());
        AddInputSystem(new SelectTargetSystem());
        AddInputSystem(new DeselectUnitSystem());
        
        AddInputSystem(new RecruitInputSystem());
        AddInputSystem(new NextUnitInputSystem());
        AddInputSystem(new SuspendUnitInputSystem());

        AddUpdateSystem(new UpdateTerrainInfoSystem());
        AddUpdateSystem(new UpdatePlayerInfoSystem());
        AddUpdateSystem(new UpdateUnitPlateSystem());
        AddUpdateSystem(new UpdateStatsInfoSystem());
        AddUpdateSystem(new UpdateHoveredLocationSystem(this));
        AddUpdateSystem(new PreviewPathSystem());
        AddUpdateSystem(new UpdateMapCursorSystem());
        AddUpdateSystem(new UpdateCameraOperatorSystem());
        AddUpdateSystem(new UpdateHoveredUnitSystem());
        AddUpdateSystem(new MoveUnitSystem());

        AddEventSystem<FocusCameraEvent>(new FocusCameraEventSystem());
        AddEventSystem<UpdateMapEvent>(new UpdateMapEventSystem());
        AddEventSystem<UpdateTerrainMeshEvent>(new UpdateTerrainMeshEventSystem());
        AddEventSystem<UpdateTerrainFeaturePopulatorEvent>(new UpdateTerrainFeaturePopulatorEventSystem());
        AddEventSystem<LoadMapEvent>(new LoadMapEventSystem());
        AddEventSystem<DespawnMapEvent>(new DespawnMapEventSystem());
        AddEventSystem<SpawnMapEvent>(new SpawnMapEventSystem(this));
        AddEventSystem<SpawnPlayersEvent>(new SpawnPlayersEventSystem());
        AddEventSystem<SpawnPlayerEvent>(new SpawnPlayerEventSystem());
        AddEventSystem<SpawnUnitEvent>(new SpawnUnitEventSystem(this));
        AddEventSystem<RecruitUnitEvent>(new RecruitUnitEventSystem(this));
        AddEventSystem<UnitHoveredEvent>(new UnitHoveredEventSystem());
        AddEventSystem<UnitDeselectedEvent>(new UnitDeselectedEventSystem());
        AddEventSystem<UnitSelectedEvent>(new UnitSelectedEventSystem());
        AddEventSystem<MoveUnitEvent>(new MoveUnitEventSystem());
        AddEventSystem<HighlightLocationEvent>(new HighlightLocationsEventSystem());
        AddEventSystem<DamageEvent>(new DamageEventSystem());
        AddEventSystem<MissEvent>(new MissEventSystem());
        AddEventSystem<GainExperienceEvent>(new GainExperienceEventSystem());
        AddEventSystem<AdvanceEvent>(new AdvanceEventSystem());
        AddEventSystem<DeathEvent>(new DeathEventSystem());
        AddEventSystem<CaptureVillageEvent>(new CaptureVillageEventSystem(this));
        AddEventSystem<SpawnFloatingLabelEvent>(new SpawnFloatingLabelEventSystem());
        AddEventSystem<TurnEndEvent>(new TurnEndEventSystem());
        AddEventSystem<ChangeDaytimeEvent>(new ChangeDaytimeEventSystem());
        AddEventSystem<CheckVictoryConditionEvent>(new CheckVictoryConditionEventSystem());

        AddDestroySystem(new DespawnCameraOperatorSystem());
    }

    public override void Enter(GameStateController gameStates)
    {
        _socket = _world.GetResource<ISocket>();
        _socket.ReceivedMatchState += OnReceivedMatchState;

        _match = _world.GetResource<IMatch>();
        var matchPlayers = _world.GetResource<MatchPlayers>();

        _world.AddResource(new Commander());

        _world.AddResource(new Scenario(matchPlayers.Array.Length));
        
        _world.AddResource(Data.Instance.Schedules["DefaultSchedule"].Instantiate<Schedule>());

        var hudView = Scenes.Instance.HudView.Instantiate<HudView>();
        hudView.Connect("TurnEndButtonPressed", new Callable(this, nameof(OnTurnEndButtonPressed)));
        AddChild(hudView);

        _world.AddResource(hudView);

        _world.Spawn().Add(new LoadMapEvent(_mapName));
        _world.Spawn().Add(new SpawnPlayersEvent(_factions));
        _world.Spawn().Add(new TurnEndEvent());
    }

    public override void Exit(GameStateController gameStates)
    {
        _socket.ReceivedMatchState -= OnReceivedMatchState;
        _socket.LeaveMatchAsync(_match);

        _world.RemoveResource<IMatch>();
        _world.RemoveResource<LocalPlayer>();
        _world.RemoveResource<MatchPlayers>();
        
        _world.RemoveResource<Commander>();
        _world.RemoveResource<Scenario>();
        _world.RemoveResource<Schedule>();

        _world.RemoveResource<HudView>();
        _world.Spawn().Add(new DespawnMapEvent());
    }

    public override void Input(GameStateController gameStates, Godot.InputEvent e)
    {
        if (e.IsActionPressed("ui_cancel"))
        {
            gameStates.PopState();
        }
    }

    public void OnTurnEndButtonPressed()
    {
        var opCode = (int)NetworkOperation.TurnEnd;
        var state = new TurnEndMessage();
        _socket.SendMatchStateAsync(_match.Id, opCode, state.ToJson());
        _world.Spawn().Add(new TurnEndEvent());
    }

    private void OnReceivedMatchState(IMatchState state)
    {
        GD.Print("Reseived Match State");
        CallDeferred(nameof(ProcessMatchStateChance), new MarshallableState { State = state });
    }

    private void ProcessMatchStateChance(MarshallableState marshallableState)
    {
        var state = marshallableState.State;
        var gameStateController = _world.GetResource<GameStateController>();

        var enc = System.Text.Encoding.UTF8;
        var data = (string)enc.GetString(state.State);
        var operation = (NetworkOperation)state.OpCode;

        GD.Print($"Network Operation: {operation.ToString()}\nJSON: {data}");

        switch (operation)
        {
            case NetworkOperation.TurnEnd:
                {
                    _world.Spawn().Add(new TurnEndEvent());
                    break;
                }
            case NetworkOperation.MoveUnit:
                {
                    var message = JsonParser.FromJson<MoveUnitMessage>(data);
                    _world.Spawn().Add(new MoveUnitEvent() { From = message.From.Cube(), To = message.To.Cube() });
                    break;
                }
            case NetworkOperation.RecruitUnit:
                {
                    var map = _world.GetResource<Map>();
                    var message = JsonParser.FromJson<RecruitUnitMessage>(data);
                    var unitType = Data.Instance.Units[message.UnitTypeId].Instantiate<UnitType>();
                    var locEntity = map.Locations.Get(message.Coords.Cube());
                    _world.Spawn().Add(new RecruitUnitEvent(message.Side, unitType, locEntity));
                    break;
                }
            case NetworkOperation.AttackUnit:
                {
                    var map = _world.GetResource<Map>();
                    var commander = _world.GetResource<Commander>();

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

                    gameStateController.PushState(new CommanderState(_world));
                    break;
                }
        }
    }
}