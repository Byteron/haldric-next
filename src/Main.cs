using Godot;

public partial class Main : Node3D
{
	public override void _Ready()
	{
		var gameStateController = new GameStateController();
		gameStateController.Name = "GameStateController";
		AddChild(gameStateController);
		
		gameStateController.PushState(new ApplicationState());
	}

	public override void _ExitTree()
	{
		// World.Destroy();
	}
}
