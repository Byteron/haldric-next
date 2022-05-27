using RelEcs;
using Godot;

namespace Haldric.Wdk
{
    public partial class MobilityTrait : Trait
    {
        [Export] TerrainType _terrainType = TerrainType.Flat;
        [Export] int _cost = 1;

        public override void Apply(Entity unitEntity)
        {
            var mobility = unitEntity.Get<Mobility>();
            mobility.Dict.Add(_terrainType, _cost);
        }
    }
}
