using Godot;
using World = RelEcs.World;

public partial class Main : Node3D
{
    readonly World _world = new();
    Features _features;
    
    public override void _Ready()
    {
        _features = new Features(_world);

		_world.AddElement(GetTree());
		
		InitFeatures();   
    }
    
	public override void _Process(double delta)
    {
        _features.Update();
    }

	void InitFeatures()
	{
		_features.Init();
	}

}
