using Godot;
using System.Collections.Generic;
using RelEcs;

public class TerrainBuilder
{
    World _world;

    Entity _entity;
    Identity _identity;


    public TerrainBuilder(World world)
    {
        _world = world;
    }

    public TerrainBuilder CreateBase()
    {
        _entity = _world.Spawn();
        _identity = _entity.Identity;
        _world.AddComponent<IsBaseTerrain>(_identity);
        _world.AddComponent(_identity, new ElevationOffset());
        return this;
    }
    public TerrainBuilder CreateOverlay()
    {
        _entity = _world.Spawn();
        _identity = _entity.Identity;
        _world.AddComponent<IsOverlayTerrain>(_identity);
        return this;
    }

    public TerrainBuilder WithCode(string code)
    {
        _world.AddComponent(_identity, new TerrainCode { Value = code });
        return this;
    }

    public TerrainBuilder WithTypes(List<TerrainType> types)
    {
        _world.AddComponent(_identity, new TerrainTypes(types));
        return this;
    }

    public TerrainBuilder WithElevationOffset(float elevationOffset)
    {
        _world.GetComponent<ElevationOffset>(_identity).Value = elevationOffset;
        return this;
    }

    public TerrainBuilder WithRecruitFrom()
    {
        _world.AddComponent<CanRecruitFrom>(_identity);
        return this;
    }

    public TerrainBuilder WithRecruitTo()
    {
        _world.AddComponent<CanRecruitTo>(_identity);
        return this;
    }

    public TerrainBuilder WithGivesIncome()
    {
        _world.AddComponent<GivesIncome>(_identity);
        return this;
    }

    public TerrainBuilder WithIsCapturable()
    {
        _world.AddComponent<IsCapturable>(_identity);
        return this;
    }

    public TerrainBuilder WithHeals()
    {
        _world.AddComponent<Heals>(_identity);
        return this;
    }

    public TerrainBuilder WithNoLighting()
    {
        _world.AddComponent<NoLighting>(_identity);
        return this;
    }

    public Entity Build()
    {
        return _entity;
    }
}
