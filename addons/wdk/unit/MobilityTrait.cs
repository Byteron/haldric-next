using RelEcs;
using Godot;

namespace Haldric.Wdk
{
    public partial class MobilityTrait : Trait
    {
        [Export] TerrainType TerrainType = TerrainType.Flat;
        [Export] int Cost = 1;

        public override void Apply(Entity unitEntity)
        {
            var mobility = unitEntity.Get<Mobility>();
            mobility.Dict.Add(TerrainType, Cost);
        }
    }
}
