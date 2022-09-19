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
        public World World { get; set; }

        public void Run()
        {
            var tree = this.GetTree();

            var camera = Scenes.Instantiate<CameraOperator>();
            this.AddElement(camera);
            tree.CurrentScene.AddChild(camera);

            var view = Scenes.Instantiate<EditorView>();
            view.CreateButton.Pressed += OnCreateButtonPressed;
            view.LoadButton.Pressed += OnLoadButtonPressed;
            view.SaveButton.Pressed += OnSaveButtonPressed;
            view.ToolsTab.TabChanged += OnToolsTabChanged;
            
            this.AddElement(view);
            tree.CurrentScene.AddChild(view);
            
            var selectedTerrain = new SelectedTerrain();
            var data = this.GetElement<TerrainData>();
            selectedTerrain.Entity = data.TerrainEntities["Gg"];
        
            foreach (var (code, entity) in data.TerrainEntities)
            {
                var button = new Button();
                button.CustomMinimumSize = new Vector2i(50, 50);
                button.Text = code;
                button.Connect("pressed", new Callable(() => { selectedTerrain.Entity = entity; }));
                view.Terrains.AddChild(button);
            }
            
            this.AddElement(selectedTerrain);
            
            this.SpawnSchedule("DefaultSchedule", 1);
            this.SpawnMap(32, 32);
            this.UpdateTerrainMesh();
            this.UpdateTerrainProps();
        }
        
        void OnToolsTabChanged(long index)
        {
            var view = this.GetElement<EditorView>();
            
            view.Mode = index switch
            {
                0 => EditorMode.Terrain,
                1 => EditorMode.Player,
                _ => view.Mode
            };
        }
        
        void OnCreateButtonPressed()
        {
            this.Send(new CreateMapTrigger());
        }

        void OnSaveButtonPressed()
        {
            this.Send(new SaveMapTrigger());
        }

        void OnLoadButtonPressed()
        {
            this.Send(new LoadMapTrigger());
        }
    }

    class UpdateSystem : ISystem
    {
        public World World { get; set; }
        
        public void Run()
        {
            var view = this.GetElement<EditorView>();
            
            foreach (var t in this.Receive<CreateMapTrigger>())
            {
                if (view.WidthTextEdit.Text.IsValidInteger() && view.HeightTextEdit.Text.IsValidInteger())
                {
                    var width = int.Parse(view.WidthTextEdit.Text);
                    var height = int.Parse(view.HeightTextEdit.Text);

                    this.DespawnMap();
                    this.SpawnMap(width, height);
                }
                else
                {
                    GD.PushWarning("Please specify valid map size!");
                }
            }
            
            foreach (var t in this.Receive<LoadMapTrigger>())
            {
                if (string.IsNullOrEmpty(view.MapNameTextEdit.Text))
                {
                    GD.PushWarning("Invalid Map Name: Please specify a Map Name");
                    return;
                }

                if (view.MapNameTextEdit.Text.IsValidIdentifier())
                {
                    var scenarioData = this.GetElement<ScenarioData>();

                    this.DespawnMap();
                    var mapData = scenarioData.Maps[view.MapNameTextEdit.Text];
                    this.SpawnMap(mapData);
                }
                else
                {
                    GD.PushWarning("Invalid Map Name: Not a Valid Identifier");
                }
            }
            
            foreach (var t in this.Receive<SaveMapTrigger>())
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
        public World World { get; set; }

        public void Run()
        {
            this.RemoveElement<SelectedTerrain>();
            this.GetElement<EditorView>().QueueFree();
            this.RemoveElement<EditorView>();
            this.GetElement<CameraOperator>().QueueFree();
            this.RemoveElement<CameraOperator>();
            this.GetElement<Schedule>().QueueFree();
            this.RemoveElement<Schedule>();
        }
    }
}