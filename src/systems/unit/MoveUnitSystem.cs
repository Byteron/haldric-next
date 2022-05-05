using Godot;
using RelEcs;
using RelEcs.Godot;
using Nakama;
using Nakama.TinyJson;

public class MoveUnitSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<HoveredLocation>(out var hoveredLocation))
        {
            return;
        }

        if (!commands.TryGetElement<SelectedLocation>(out var selectedLocation))
        {
            return;
        }

        var commander = commands.GetElement<Commander>();
        var map = commands.GetElement<Map>();

        var hoveredLocEntity = hoveredLocation.Entity;

        if (!hoveredLocEntity.IsAlive || hoveredLocEntity.Has<HasUnit>())
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

            commands.Send(new UnitDeselectedEvent());
            commands.Send(new MoveUnitEvent { From = fromCoords.Cube(), To = toCoords.Cube() });

            if (!commands.TryGetElement<ISocket>(out var socket))
            {
                return;
            }

            if (!commands.TryGetElement<IMatch>(out var match))
            {
                return;
            }

            var message = new MoveUnitMessage { From = fromCoords, To = toCoords };
            var json = message.ToJson();

            socket.SendMatchStateAsync(match.Id, (int)NetworkOperation.MoveUnit, json);
        }
    }
}