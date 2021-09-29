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

        var cursorQuery = world.Query<MapCursor>().End();

        foreach (var cursorEntityId in cursorQuery)
        {
            var cursorEntity = world.Entity(cursorEntityId);

            var locEntity = cursorEntity.Get<HoveredLocation>().Entity;
            
            if (!locEntity.IsAlive())
            {
                return;
            }

            if (Input.IsActionJustPressed("select_unit"))
            {
                if (locEntity.Has<HasUnit>())
                {
                    cursorEntity.Add(new HasLocation(locEntity));

                    var unitEntity = locEntity.Get<HasUnit>().Entity;
                    var id = unitEntity.Get<Id>().Value;
                    var hp = unitEntity.Get<Attribute<Health>>().Value;
                    var xp = unitEntity.Get<Attribute<Experience>>().Value;
                    var mp = unitEntity.Get<Attribute<Moves>>().Value;
                    var s = string.Format("Id: {0}\nHP: {1}\nXP: {2}\nMP: {3}", id, hp, xp, mp);

                    _parent.GetTree().CallGroup("UnitLabel", "set", "text", s);
                }
            }

            if (Input.IsActionJustPressed("deselect_unit"))
            {
                cursorEntity.Remove<HasLocation>();
                _parent.GetTree().CallGroup("UnitLabel", "set", "text", "");
            }
        }

    }
}