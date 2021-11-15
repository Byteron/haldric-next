using Godot;
using Godot.Collections;

public partial class Sfx : Node
{
    public static Sfx Instance { get; private set; }

    Dictionary<string, AudioStreamPlayer> _sfx = new Dictionary<string, AudioStreamPlayer>();

    public override void _Ready()
    {
        Instance = this;

        foreach (Node child in GetChildren())
        {
            if (child is AudioStreamPlayer player)
            {
                _sfx.Add(player.Name, player);
            }
        }
    }

    public void Play(string sfxName)
    {
        if (!_sfx.ContainsKey(sfxName))
        {
            return;
        }

        _sfx[sfxName].Play();
    }
}
