using Godot;
using Godot.Collections;
using Bitron.Ecs;

public class UnitFactory
{
    static UnitBuilder _builder = new UnitBuilder();

    public static EcsEntity CreateFromUnitType(UnitType unitType, UnitView unitView)
    {
        var unit = _builder
            .Create()
            .WithId(unitType.Attributes.Id)
            .WithHealth(unitType.Attributes.Health)
            .WithExperience(unitType.Attributes.Experience)
            .WithMoves(unitType.Attributes.Moves)
            .WithView(unitView)
            .Build();

        return unit;
    }
}