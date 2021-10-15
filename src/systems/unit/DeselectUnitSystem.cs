using Godot;
using Bitron.Ecs;

public class DeselectUnitSystem : IEcsSystem
{
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

            if (Input.IsActionJustPressed("deselect_unit"))
            {
                var hoverEntity = world.Entity(hoverEntityId);
                
                hoverEntity.Remove<HasLocation>();

                var hudView = world.GetResource<HudView>();
                hudView.UnitLabel.Text = "";

                var shaderData = world.GetResource<ShaderData>();
                shaderData.ResetVisibility(true);
                shaderData.Apply();
            }
        }
    }
}