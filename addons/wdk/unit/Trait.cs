using Godot;
using RelEcs;

namespace Haldric.Wdk
{
    public partial class Trait : Node
    {
        [Export] string _id = "";
        [Export(PropertyHint.MultilineText)] string _description = "";

        public virtual void Apply(Entity unitEntity) { }
    }
}
