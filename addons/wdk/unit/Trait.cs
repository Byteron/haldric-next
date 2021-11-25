using Godot;
using Bitron.Ecs;

namespace Haldric.Wdk
{
    public partial class Trait : Node
    {
        [Export] string Id = "";
        [Export(PropertyHint.MultilineText)] string Description = "";

        public virtual void Apply(EcsEntity unitEntity) { }
    }
}
