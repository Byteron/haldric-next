using RelEcs;

public partial class MenuState : GameState
{
    public override void Init()
    {
        Enter.Add(new EnterSystem());
        Update.Add(new UpdateDebugInfoSystem());
        Exit.Add(new ExitSystem());
    }

    class EnterSystem : ISystem
    {
        World world;

        public void Run(World world)
        {
            this.world = world;
            
            var canvas = world.GetElement<Canvas>();
            var layer = canvas.GetCanvasLayer(1);

            var menu = Scenes.Instantiate<MainMenuView>();
            layer.AddChild(menu);

            menu.TestButton.Pressed += OnTestButtonPressed;
            menu.LobbyButton.Pressed += OnLobbyButtonPressed;
            menu.EditorButton.Pressed += OnEditorButtonPressed;
            menu.QuitButton.Pressed += OnQuitButtonPressed;

            world.AddElement(menu);
        }

        void OnLobbyButtonPressed()
        {
        }

        void OnEditorButtonPressed()
        {
            world.ChangeState(new EditorState());
        }

        void OnTestButtonPressed()
        {
            world.ChangeState(new TestState());
        }

        void OnQuitButtonPressed()
        {
            world.GetTree().Quit();
        }
    }

    class ExitSystem : ISystem
    {
        public World World { get; set; }

        public void Run(World world)
        {
            world.GetElement<MainMenuView>().QueueFree();
        }
    }
}