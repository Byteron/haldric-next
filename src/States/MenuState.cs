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
        public World World { get; set; }

        public void Run()
        {
            var canvas = this.GetElement<Canvas>();
            var layer = canvas.GetCanvasLayer(1);

            var menu = Scenes.Instantiate<MainMenuView>();
            layer.AddChild(menu);

            menu.TestButton.Pressed += OnTestButtonPressed;
            menu.LobbyButton.Pressed += OnLobbyButtonPressed;
            menu.EditorButton.Pressed += OnEditorButtonPressed;
            menu.QuitButton.Pressed += OnQuitButtonPressed;

            this.AddElement(menu);
        }

        void OnLobbyButtonPressed()
        {
        }

        void OnEditorButtonPressed()
        {
            this.ChangeState(new EditorState());
        }

        void OnTestButtonPressed()
        {
            this.ChangeState(new TestState());
        }

        void OnQuitButtonPressed()
        {
            this.GetTree().Quit();
        }
    }

    class ExitSystem : ISystem
    {
        public World World { get; set; }

        public void Run()
        {
            this.GetElement<MainMenuView>().QueueFree();
        }
    }
}