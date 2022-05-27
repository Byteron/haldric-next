using RelEcs;

public class UnitDeselectedEvent { }

public class UnitDeselectedEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((UnitDeselectedEvent e) =>
        {
            if (!commands.HasElement<SelectedLocation>()) return;
            
            commands.RemoveElement<SelectedLocation>();

            var terrainHighlighter = commands.GetElement<TerrainHighlighter>();
            terrainHighlighter.Clear();

            var unitPanel = commands.GetElement<UnitPanel>();
            unitPanel.UpdateInfo("");
        });
    }
}