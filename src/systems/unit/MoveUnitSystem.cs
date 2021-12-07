using Godot;
using Bitron.Ecs;
using Nakama;
using Nakama.TinyJson;

public class MoveUnitSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        if (!world.TryGetResource<HoveredLocation>(out var hoveredLocation))
        {
            return;
        }

        if (!world.TryGetResource<SelectedLocation>(out var selectedLocation))
        {
            return;
        }

        var commander = world.GetResource<Commander>();
        var map = world.GetResource<Map>();

        var hoveredLocEntity = hoveredLocation.Entity;

        if (!hoveredLocEntity.IsAlive() || hoveredLocEntity.Has<HasUnit>())
        {
            return;
        }

        var selectedLocEntity = selectedLocation.Entity;

        if (selectedLocEntity.Get<Coords>().Cube() == hoveredLocEntity.Get<Coords>().Cube())
        {
            return;
        }

        if (Input.IsActionJustPressed("select_unit"))
        {
            ref var fromCoords = ref selectedLocEntity.Get<Coords>();
            ref var toCoords = ref hoveredLocEntity.Get<Coords>();

            world.Spawn().Add(new UnitDeselectedEvent());
            world.Spawn().Add(new MoveUnitEvent { From = fromCoords.Cube(), To = toCoords.Cube() });

            if (!world.TryGetResource<ISocket>(out var socket))
            {
                return;
            }

            if (!world.TryGetResource<IMatch>(out var match))
            {
                return;
            }

            var message = new MoveUnitMessage { From = fromCoords, To = toCoords };
            var json = message.ToJson();

            socket.SendMatchStateAsync(match.Id, (int)NetworkOperation.MoveUnit, json);
        }
    }
}