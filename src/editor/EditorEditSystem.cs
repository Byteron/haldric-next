using Godot;
using Leopotam.Ecs;

public struct Editor {}

public class EditorEditSystem : IEcsInitSystem, IEcsRunSystem
{
    EcsWorld _world;

    Node3D _parent;

    EcsFilter<HoveredCoords> _hoveredCoords;
    EcsFilter<Locations> _locations;

    EcsFilter<Editor> _editors;

    public EditorEditSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Init()
    {
        var editorEntity = _world.NewEntity();

        var editorView = Scenes.Instance.EditorView.Instance<EditorView>();
        _parent.AddChild(editorView);

        editorEntity.Replace(new NodeHandle<EditorView>(editorView));
        editorEntity.Get<Editor>();
    }

    public void Run()
    {
        foreach(var i in _locations)
        {
            if (_hoveredCoords.IsEmpty() || _locations.IsEmpty() || _editors.IsEmpty())
            {
                return;
            }
            
            var editorEntity = _editors.GetEntity(0);

            ref var editorView = ref editorEntity.Get<NodeHandle<EditorView>>().Node;

            ref var locations = ref _locations.GetEntity(0).Get<Locations>();
            ref var hoveredCoords = ref _hoveredCoords.GetEntity(0).Get<HoveredCoords>();

            if (Input.IsActionPressed("editor_select"))
            {
                foreach (var cube in Hex.GetCellsInRange(hoveredCoords.Coords.Cube, editorView.BrushSize))
                {
                    if (!locations.Has(cube))
                    {
                        continue;
                    }

                    var locEntity = locations.Get(cube);
                    EditLocation(editorEntity, locEntity);
                }

                SendUpdateMapEvent();
            }
            
        }
    }

    private void EditLocation(EcsEntity editorEntity, EcsEntity locEntity)
    {
        ref var editorView = ref editorEntity.Get<NodeHandle<EditorView>>().Node;

        ref var elevation = ref locEntity.Get<Elevation>();

        elevation.Level = editorView.Elevation;
    }

    private void SendUpdateMapEvent()
    {
        var updateMapEventEntity = _world.NewEntity();
        updateMapEventEntity.Get<UpdateMapEvent>();
    }
}