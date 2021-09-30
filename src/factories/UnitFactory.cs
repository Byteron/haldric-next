using Godot;
using Godot.Collections;
using Bitron.Ecs;

public class UnitFactory
{
    static UnitBuilder _builder = new UnitBuilder();

    public static EcsEntity CreateFromDict(Dictionary dict)
    {
        var unit = _builder
            .Create()
            .WithId((string)dict["id"])
            .WithHealth((int)(float)dict["hp"])
            .WithExperience((int)(float)dict["xp"])
            .WithMoves((int)(float)dict["mp"])
            .WithView((string)dict["view"])
            .Build();

        return unit;
    }
}