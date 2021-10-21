using System.Collections.Generic;
using System.Linq;
using Bitron.Ecs;
using Godot;

public partial class MoveUnitCommand : Command
{
    private Path _path;
    private EcsEntity _unitEntity;

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
            GD.Print("No Unit To Move");
            return;
        }

        // target loc already occupied
        if (_path.Destination.Has<HasUnit>())
        {
            GD.Print("Destination Already Occupied");
            return;
        }

        _unitEntity = _path.Start.Get<HasUnit>().Entity;
        var unitView = _unitEntity.Get<NodeHandle<UnitView>>().Node;
        
        ref var unitCoords = ref _unitEntity.Get<Coords>();
        ref var unitMoves = ref _unitEntity.Get<Attribute<Moves>>();

        var tween = Main.Instance.GetTree().CreateTween();

        foreach(var checkpointLocEntity in _path.Checkpoints)
        {
            ref var targetCoords = ref checkpointLocEntity.Get<Coords>();
            ref var targetElevation = ref checkpointLocEntity.Get<Elevation>();

            var newPos = targetCoords.World;
            newPos.y = targetElevation.Height;
            tween.TweenProperty(unitView, "position", newPos, 0.25f);
        }

        tween.TweenCallback(new Callable(this, "OnUnitMoveFinished"));
        tween.Play();

        _path.Start.Remove<HasUnit>();
        _path.Destination.Add(new HasUnit(_unitEntity));
        
        unitCoords = _path.Destination.Get<Coords>();
        unitMoves.Empty();
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
        Main.Instance.World.Spawn().Add(new UnitSelectedEvent(_unitEntity));
    }
}