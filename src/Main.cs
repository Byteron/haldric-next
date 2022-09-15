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
		_world.AddElement(_features);
		
		InitFeatures();   
		
		_features.EnableFeature<AppFeature>();
    }
    
	public override void _Process(double delta)
    {
        _features.Update();
    }

	void InitFeatures()
	{
		_features.InitFeature<AppFeature>();
		_features.InitFeature<MenuFeature>();
		
		_features.Init();
	}

}
