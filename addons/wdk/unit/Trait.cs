using Godot;
using RelEcs;

namespace Haldric.Wdk
{
    public partial class Trait : Node
    {
        [Export] string Id = "";
        [Export(PropertyHint.MultilineText)] string Description = "";

        public virtual void Apply(Entity unitEntity) { }
    }
}
