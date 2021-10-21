using System.Collections.Generic;
using System.Linq;
using Bitron.Ecs;
using Godot;
using Haldric.Wdk;

public partial class MoveUnitCommand : Command
{
    private Path _path;
    private EcsEntity _unitEntity;
    private UnitView _unitView;

    public MoveUnitCommand(Path path)
    {
        IsRevertable = true;
        _path = path;
    }

    public override void Execute()
    {
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
        _unitView = _unitEntity.Get<NodeHandle<UnitView>>().Node;

        ref var unitCoords = ref _unitEntity.Get<Coords>();
        ref var unitActions = ref _unitEntity.Get<Attribute<Actions>>();

        var tween = Main.Instance.GetTree().CreateTween();

        foreach (var checkpointLocEntity in _path.Checkpoints)
        {
            ref var targetCoords = ref checkpointLocEntity.Get<Coords>();
            ref var targetElevation = ref checkpointLocEntity.Get<Elevation>();

            var newPos = targetCoords.World;
            newPos.y = targetElevation.Height;
            tween.TweenCallback(new Callable(this, "OnUnitStepFinished"));
            tween.TweenProperty(_unitView, "position", newPos, 0.2f);
        }

        tween.TweenCallback(new Callable(this, "OnUnitMoveFinished"));
        tween.Play();

        _path.Start.Remove<HasUnit>();
        _path.Destination.Add(new HasUnit(_unitEntity));

        unitCoords = _path.Destination.Get<Coords>();

        if (IsReverted)
        {
            unitActions.Increase(_path.Checkpoints.Count);
        }
        else
        {
            unitActions.Decrease(_path.Checkpoints.Count);
        }
    }

    public override void Revert()
    {
        _unitEntity = default;

        var temp = _path.Start;
        _path.Start = _path.Destination;
        _path.Destination = temp;

        _path.Checkpoints = new Queue<EcsEntity>(_path.Checkpoints.Reverse());
        _path.Checkpoints.Dequeue();
        _path.Checkpoints.Enqueue(_path.Destination);

        IsReverted = true;
    }

    private void OnUnitMoveFinished()
    {
        if (_unitEntity.Get<Attribute<Actions>>().Value > 0)
        {
            Main.Instance.World.Spawn().Add(new UnitSelectedEvent(_unitEntity));
        }
        else
        {
            Main.Instance.World.Spawn().Add(new UnitDeselectedEvent());
        }
    }

    private int index;

    private void OnUnitStepFinished()
    {
        if (index == _path.Checkpoints.Count)
        {
            index = 0;
        }
        else
        {
            var coords = _path.Checkpoints.ToArray()[index].Get<Coords>();
            var pos = coords.World;
            _unitView.LookAt(coords.World, Vector3.Up);
            _unitView.Rotation = new Vector3(0f, _unitView.Rotation.y, 0f);
            index += 1;
        }

    }
}