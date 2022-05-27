using RelEcs;
using RelEcs.Godot;
using Godot;
using Haldric.Wdk;

public partial class MoveUnitCommand : Resource, ICommandSystem
{
    Path _path;

    Entity _unitEntity;
    Entity _targetLocEntity;

    UnitView _unitView;

    Tween _tween;

    Commands _commands;

    public MoveUnitCommand(Path path)
    {
        // IsRevertable = true;
        _path = path;
    }

    public void Run(Commands commands)
    {
        this._commands = commands;

        // source location does not have a unit to move
        if (!_path.Start.Has<HasUnit>())
        {
            GD.PushWarning("No Unit To Move");
            return;
        }

        // target loc already occupied
        if (_path.Destination.Has<HasUnit>())
        {
            GD.PushWarning("Destination Already Occupied");
            return;
        }

        _unitEntity = _path.Start.Get<HasUnit>().Entity;
        _unitView = _unitEntity.Get<UnitView>();

        var unitCoords = _unitEntity.Get<Coords>();
        _tween = commands.GetElement<SceneTree>().CreateTween();

        foreach (var checkpointLocEntity in _path.Checkpoints)
        {
            var targetCoords = checkpointLocEntity.Get<Coords>();
            var targetElevation = checkpointLocEntity.Get<Elevation>();

            var terrainEntity = checkpointLocEntity.Get<HasBaseTerrain>().Entity;
            var elevationOffset = terrainEntity.Get<ElevationOffset>();

            var newPos = targetCoords.World();
            newPos.y = targetElevation.Height + elevationOffset.Value;

            _tween.TweenCallback(new Callable(this, "OnUnitStepFinished"));
            _tween.TweenProperty(_unitView, "position", newPos, 0.2f);
        }

        _tween.TweenCallback(new Callable(this, "OnUnitMoveFinished"));
        _tween.Play();

        _path.Start.Remove<HasUnit>();

        _path.Destination.Add(new HasUnit { Entity = _unitEntity });
        unitCoords.X = _path.Destination.Get<Coords>().X;
        unitCoords.Z = _path.Destination.Get<Coords>().Z;
    }

    public void Revert()
    {
        // _unitEntity = default;
        // _targetLocEntity = default;

        // var temp = _path.Start;
        // _path.Start = _path.Destination;
        // _path.Destination = temp;

        // _path.Checkpoints = new Queue<Entity>(_path.Checkpoints.Reverse());
        // _path.Checkpoints.Dequeue();
        // _path.Checkpoints.Enqueue(_path.Destination);

        // IsReverted = true;
        // IsDone = false;
    }

    void OnUnitMoveFinished()
    {
        var side = _unitEntity.Get<Side>();
        var moves = _unitEntity.Get<Attribute<Moves>>();

        if (_targetLocEntity.Has<Village>())
        {
            if (_targetLocEntity.Has<IsCapturedBySide>())
            {
                var captured = _targetLocEntity.Get<IsCapturedBySide>();

                if (captured.Value != side.Value)
                {
                    moves.Empty();
                    _commands.Send(new CaptureVillageTrigger(_targetLocEntity, _unitEntity.Get<Side>().Value));
                }
            }
            else
            {
                moves.Empty();
                _commands.Send(new CaptureVillageTrigger(_targetLocEntity, _unitEntity.Get<Side>().Value));
            }
        }

        if (_targetLocEntity.Has<IsInZoc>()) moves.Empty();

        IsDone = true;
    }

    int _index;

    public bool IsDone { get; set; }
    public bool IsRevertible { get; set; }
    public bool IsReverted { get; set; }

    void OnUnitStepFinished()
    {
        if (_index == _path.Checkpoints.Count)
        {
            _index = 0;
        }
        else
        {
            var moves = _unitEntity.Get<Attribute<Moves>>();
            var mobility = _unitEntity.Get<Mobility>();

            if (_targetLocEntity is not null && _targetLocEntity.IsAlive && _targetLocEntity.Has<IsInZoc>())
            {
                _tween.Stop();
                OnUnitMoveFinished();
                return;
            }

            _targetLocEntity = _path.Checkpoints.ToArray()[_index];

            var coords = _targetLocEntity.Get<Coords>();

            _unitView.LookAt(coords.World(), Vector3.Up);
            _unitView.Rotation = new Vector3(0f, _unitView.Rotation.y, 0f);

            var movementCosts = TerrainTypes.FromLocEntity(_targetLocEntity).GetMovementCost(mobility);

            if (IsReverted) moves.Increase(movementCosts);
            else moves.Decrease(movementCosts);

            _index += 1;
        }
    }
}