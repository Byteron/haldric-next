using Godot;
using Godot.Collections;
using Leopotam.Ecs;

public class UnitFactory
{
    static UnitBuilder _builder = new UnitBuilder();

    public static EcsEntity CreateFromJSON(string jsonString)
    {
        var json = new JSON();
        json.Parse(jsonString);
        var dict = json.GetData() as Dictionary;

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