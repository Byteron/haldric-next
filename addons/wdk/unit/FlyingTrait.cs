using Bitron.Ecs;

namespace Haldric.Wdk
{   
    public partial class FlyingTrait : Trait
    {
        public override void Apply(EcsEntity unitEntity)
        {
            ref var mobility = ref unitEntity.Get<Mobility>();
            mobility.Dict.Add(TerrainType.Flat, 1);
            mobility.Dict.Add(TerrainType.Settled, 1);
            mobility.Dict.Add(TerrainType.Fortified, 1);
            mobility.Dict.Add(TerrainType.Forested, 1);
            mobility.Dict.Add(TerrainType.Infested, 1);
            mobility.Dict.Add(TerrainType.Rough, 1);
            mobility.Dict.Add(TerrainType.Rocky, 1);
            mobility.Dict.Add(TerrainType.Reefy, 1);
            mobility.Dict.Add(TerrainType.ShallowWaters, 1);
            mobility.Dict.Add(TerrainType.DeepWaters, 1);
            mobility.Dict.Add(TerrainType.Unwalkable, 1);
        }
    }
}
