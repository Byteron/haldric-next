using Godot;
using RelEcs;

public partial class Trait : Node
{
    [Export] string _id = "";
    [Export(PropertyHint.MultilineText)] string _description = "";

    public virtual void Apply(EntityBuilder entityBuilder) { }
}