using RelEcs;
using Godot;

public partial class EditorState : GameState
{
    class SaveMapTrigger {}
    class LoadMapTrigger {}
    class CreateMapTrigger {}
    
    public override void Init()
    {
        Enter.Add(new EnterSystem());
        Exit.Add(new ExitSystem());

        Update
            .Add(new UpdateSystem())
            .Add(new EditorEditTerrainSystem())
            .Add(new EditorEditPlayerSystem())
            .Add(new UpdateCameraSystem())
            .Add(new UpdateHoveredTileSystem())
            .Add(new UpdateMapCursorSystem())
            .Add(new UpdateDebugInfoSystem());
    }

    partial class EnterSystem : RefCounted, ISystem
    {
        World world;

        public void Run(World world)
        {
            this.world = world;

            var tree = world.GetTree();

            var camera = Scenes.Instantiate<CameraOperator>();
            world.AddElement(camera);
            tree.CurrentScene.AddChild(camera);

            var view = Scenes.Instantiate<EditorView>();
            view.CreateButton.Pressed += OnCreateButtonPressed;
            view.LoadButton.Pressed += OnLoadButtonPressed;
            view.SaveButton.Pressed += OnSaveButtonPressed;
            view.ToolsTab.TabChanged += OnToolsTabChanged;
            
            world.AddElement(view);
            tree.CurrentScene.AddChild(view);
            
            var selectedTerrain = new SelectedTerrain();
            var data = world.GetElement<TerrainData>();
            selectedTerrain.Entity = data.TerrainEntities["Gg"];
        
            foreach (var (code, entity) in data.TerrainEntities)
            {
                var button = new Button();
                button.CustomMinimumSize = new Vector2i(50, 50);
                button.Text = code;
                button.Connect("pressed", new Callable(() => { selectedTerrain.Entity = entity; }));
                view.Terrains.AddChild(button);
            }
            
            world.AddElement(selectedTerrain);
            
            world.SpawnSchedule("DefaultSchedule", 1);
            world.SpawnMap(32, 32);
            world.UpdateTerrainMesh();
            world.UpdateTerrainProps();
        }
        
        void OnToolsTabChanged(long index)
        {
            var view = world.GetElement<EditorView>();
            
            view.Mode = index switch
            {
                0 => EditorMode.Terrain,
                1 => EditorMode.Player,
                _ => view.Mode
            };
        }
        
        void OnCreateButtonPressed()
        {
            world.Send(new CreateMapTrigger());
        }

        void OnSaveButtonPressed()
        {
            world.Send(new SaveMapTrigger());
        }

        void OnLoadButtonPressed()
        {
            world.Send(new LoadMapTrigger());
        }
    }

    class UpdateSystem : ISystem
    {
        public World World { get; set; }
        
        public void Run(World world)
        {
            var view = world.GetElement<EditorView>();
            
            foreach (var t in world.Receive<CreateMapTrigger>(this))
            {
                if (view.WidthTextEdit.Text.IsValidInteger() && view.HeightTextEdit.Text.IsValidInteger())
                {
                    var width = int.Parse(view.WidthTextEdit.Text);
                    var height = int.Parse(view.HeightTextEdit.Text);

                    world.DespawnMap();
                    world.SpawnMap(width, height);
                }
                else
                {
                    GD.PushWarning("Please specify valid map size!");
                }
            }
            
            foreach (var t in world.Receive<LoadMapTrigger>(this))
            {
                if (string.IsNullOrEmpty(view.MapNameTextEdit.Text))
                {
                    GD.PushWarning("Invalid Map Name: Please specify a Map Name");
                    return;
                }

                if (view.MapNameTextEdit.Text.IsValidIdentifier())
                {
                    var scenarioData = world.GetElement<ScenarioData>();

                    world.DespawnMap();
                    var mapData = scenarioData.Maps[view.MapNameTextEdit.Text];
                    world.SpawnMap(mapData);
                }
                else
                {
                    GD.PushWarning("Invalid Map Name: Not a Valid Identifier");
                }
            }
            
            foreach (var t in world.Receive<SaveMapTrigger>(this))
            {
                if (string.IsNullOrEmpty(view.MapNameTextEdit.Text))
                {
                    GD.PushWarning("Invalid Map Name: Please specify a Map Name");
                    return;
                }

                if (view.MapNameTextEdit.Text.IsValidIdentifier())
                {
                    // Commands.Send(new SaveMapTrigger(_mapNameTextEdit.Text));
                }
                else
                {
                    GD.PushWarning("Invalid Map Name: Not a Valid Identifier");
                }
            }
        }
    }

    class ExitSystem : ISystem
    {
        public void Run(World world)
        {
            world.RemoveElement<SelectedTerrain>();
            world.GetElement<EditorView>().QueueFree();
            world.RemoveElement<EditorView>();
            world.GetElement<CameraOperator>().QueueFree();
            world.RemoveElement<CameraOperator>();
            world.GetElement<Schedule>().QueueFree();
            world.RemoveElement<Schedule>();
        }
    }
}