using Godot;
using RelEcs;
using RelEcs.Godot;

public partial class ApplicationState : GameState
{

    public override void Init(GameStateController gameStates)
    {
        InitSystems.Add(new ApplicationStateInitSystem());
        
        gameStates.PushState(new LoadingState());
    }
}

public class ApplicationStateInitSystem : ISystem
{
    public void Run(Commands commands)
    {
        var canvas = new Canvas();
		canvas.Name = "Canvas";
		commands.GetElement<CurrentGameState>().State.AddChild(canvas);
		commands.AddElement(canvas);

		commands.AddElement(new ServerSettings
		{
			Host = "49.12.208.4",
			Port = 7350,
			Scheme = "http",
			ServerKey = "defaultkey",
		});

		commands.AddElement(new LobbySettings
		{
			RoomName = "general",
			Persistence = true,
			Hidden = false,
		});
    }
}