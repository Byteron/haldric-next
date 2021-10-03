using Godot;
using Bitron.Ecs;

public class SelectLocationSystem : IEcsSystem
{
    Node3D _parent;

    public SelectLocationSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(EcsWorld world)
    {
        var query = world.Query<HoveredLocation>().End();

        foreach (var hoverEntityId in query)
        {
            var locEntity = query.Get<HoveredLocation>(hoverEntityId).Entity;
            
            if (!locEntity.IsAlive())
            {
                return;
            }

            if (Input.IsActionJustPressed("select_unit"))
            {
                var hoverEntity = world.Entity(hoverEntityId);

                if (locEntity.Has<HasUnit>())
                {
                    if (!hoverEntity.Has<HasLocation>())
                    {
                        hoverEntity.Add(new HasLocation(locEntity));
                    }

                    var unitEntity = locEntity.Get<HasUnit>().Entity;
                    var id = unitEntity.Get<Id>().Value;
                    var hp = unitEntity.Get<Attribute<Health>>().Value;
                    var xp = unitEntity.Get<Attribute<Experience>>().Value;
                    var mp = unitEntity.Get<Attribute<Moves>>().Value;

                    var s = string.Format("Id: {0}\nHP: {1}\nXP: {2}\nMP: {3}", id, hp, xp, mp);

                    if (unitEntity.Has<Attacks>())
                    {
                        s += "\nAttacks:";
                        foreach(EcsEntity attackEntity in unitEntity.Get<Attacks>().GetList())
                        {
                            ref var attackId = ref attackEntity.Get<Id>();
                            ref var damage = ref attackEntity.Get<Damage>();
                            ref var strikes = ref attackEntity.Get<Strikes>();
                            ref var range = ref attackEntity.Get<Range>();
                            s += string.Format("\n{0} {1}x{2} ({3}) ({4})", attackId.Value, damage.Value, strikes.Value, damage.Type.ToString(), range.Value.ToString());
                        }
                    }

                    _parent.GetTree().CallGroup("UnitLabel", "set", "text", s);

                    var shaderData = world.GetResource<ShaderData>();
                    shaderData.ResetVisibility(false);

                    var coords = locEntity.Get<Coords>();

                    var cellsInRange = Hex.GetCellsInRange(coords.Cube, mp);

                    foreach(var cCell in cellsInRange)
                    {
                        var nCoords = Coords.FromCube(cCell);
                        shaderData.UpdateVisibility((int)nCoords.Offset.x, (int)nCoords.Offset.z, true);
                    }

                    shaderData.Apply();
                }
            }

            if (Input.IsActionJustPressed("deselect_unit"))
            {
                var hoverEntity = world.Entity(hoverEntityId);
                
                hoverEntity.Remove<HasLocation>();

                _parent.GetTree().CallGroup("UnitLabel", "set", "text", "");

                var shaderData = world.GetResource<ShaderData>();
                shaderData.ResetVisibility(true);
                shaderData.Apply();
            }
        }

    }
}