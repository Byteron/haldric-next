using Godot;
using System.Collections.Generic;

namespace Haldric.Wdk
{
    public abstract class TerrainLoader
    {
        public Dictionary<string, Dictionary<string, object>> TerrainDicts = new Dictionary<string, Dictionary<string, object>>();
        public Dictionary<string, Dictionary<string, TerrainGraphic>> Decorations = new Dictionary<string, Dictionary<string, TerrainGraphic>>();
        public Dictionary<string, Dictionary<string, TerrainGraphic>> DirectionalDecorations = new Dictionary<string, Dictionary<string, TerrainGraphic>>();
        public Dictionary<string, TerrainGraphic> WaterGraphics = new Dictionary<string, TerrainGraphic>();
        public Dictionary<string, TerrainGraphic> WallSegments = new Dictionary<string, TerrainGraphic>();
        public Dictionary<string, TerrainGraphic> WallTowers = new Dictionary<string, TerrainGraphic>();
        public Dictionary<string, TerrainGraphic> KeepPlateaus = new Dictionary<string, TerrainGraphic>();
        public Dictionary<string, Texture2D> TerrainTextures = new Dictionary<string, Texture2D>();
        public Dictionary<string, Texture2D> TerrainNormalTextures = new Dictionary<string, Texture2D>();
        public Dictionary<string, Texture2D> TerrainRoughnessTextures = new Dictionary<string, Texture2D>();

        private TerrainDictBuilder _terrainBuilder = new TerrainDictBuilder();

        private TerrainGraphicBuilder _terrainGraphicBuilder = new TerrainGraphicBuilder();

        public abstract void Load();

        public void NewBase(string code, List<TerrainType> types, float elevationOffset = 0)
        {
            var terrain = _terrainBuilder.CreateBase().WithCode(code).WithTypes(types).WithElevationOffset(elevationOffset).Build();
            TerrainDicts.Add(code, terrain);
        }

        public void NewShallowWater(string code, List<TerrainType> types)
        {
            var terrain = _terrainBuilder.CreateBase().WithCode(code).WithTypes(types).WithElevationOffset(Metrics.ShallowWaterOffset).Build();
            TerrainDicts.Add(code, terrain);
        }

        public void NewDeepWater(string code, List<TerrainType> types)
        {
            var terrain = _terrainBuilder.CreateBase().WithCode(code).WithTypes(types).WithElevationOffset(Metrics.DeepWaterOffset).Build();
            TerrainDicts.Add(code, terrain);
        }

        public void NewCastle(string code, List<TerrainType> types)
        {
            var terrain = _terrainBuilder.CreateBase().WithCode(code).WithTypes(types).WithRecruitTo().Build();
            TerrainDicts.Add(code, terrain);
        }

        public void NewKeep(string code, List<TerrainType> types)
        {
            var terrain = _terrainBuilder.CreateBase().WithCode(code).WithTypes(types).WithElevationOffset(Metrics.KeepOffset).WithRecruitFrom().WithRecruitTo().Build();
            TerrainDicts.Add(code, terrain);
        }

        public void NewHouses(string code, List<TerrainType> types)
        {
            var terrain = _terrainBuilder.CreateOverlay().WithCode(code).WithTypes(types).WithGivesIncome().Build();
            TerrainDicts.Add(code, terrain);
        }

        public void NewVillage(string code, List<TerrainType> types)
        {
            var terrain = _terrainBuilder.CreateOverlay().WithCode(code).WithTypes(types).WithIsCapturable().WithGivesIncome().WithHeals().Build();
            TerrainDicts.Add(code, terrain);
        }

        public void NewOverlay(string code, List<TerrainType> types)
        {
            var terrain = _terrainBuilder.CreateOverlay().WithCode(code).WithTypes(types).Build();
            TerrainDicts.Add(code, terrain);
        }

        public void AddTerrainTexture(string code, string path)
        {
            TerrainTextures.Add(code, LoadAsset<Texture2D>(path));
        }

        public void AddTerrainNormalTexture(string code, string path)
        {
            TerrainNormalTextures.Add(code, LoadAsset<Texture2D>(path));
        }

        public void AddTerrainRoughnessTexture(string code, string path)
        {
            TerrainRoughnessTextures.Add(code, LoadAsset<Texture2D>(path));
        }

        public void AddDecorationGraphic(string code, string path, string name = null)
        {
            if (!Decorations.ContainsKey(code))
            {
                Decorations.Add(code, new Dictionary<string, TerrainGraphic>());
            }

            var dict = Decorations[code];

            if (string.IsNullOrEmpty(name))
            {
                var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(LoadAsset<Mesh>(path)).Build();
                dict.Add(path, graphic);
            }
            else
            {
                if (!dict.ContainsKey(name))
                {
                    var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(LoadAsset<Mesh>(path)).Build();
                    dict.Add(name, graphic);
                }
                else
                {
                    var graphic = dict[name];
                    graphic.AddVariation(LoadAsset<Mesh>(path));
                }
            }
        }

        public void AddDirectionalDecorationGraphic(string code, string path, string name = null)
        {
            if (!DirectionalDecorations.ContainsKey(code))
            {
                DirectionalDecorations.Add(code, new Dictionary<string, TerrainGraphic>());
            }

            var dict = DirectionalDecorations[code];

            if (string.IsNullOrEmpty(name))
            {
                var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(LoadAsset<Mesh>(path)).Build();
                dict.Add(path, graphic);
            }
            else
            {
                if (!dict.ContainsKey(name))
                {
                    var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(LoadAsset<Mesh>(path)).Build();
                    dict.Add(name, graphic);
                }
                else
                {
                    var graphic = dict[name];
                    graphic.AddVariation(LoadAsset<Mesh>(path));
                }
            }
        }

        public void AddWaterGraphic(string code, string path)
        {
            var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(LoadAsset<Mesh>(path)).Build();
            WaterGraphics.Add(code, graphic);
        }

        public void AddWallSegmentGraphic(string code, string path)
        {
            var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(LoadAsset<Mesh>(path)).Build();
            WallSegments.Add(code, graphic);
        }

        public void AddWallTowerGraphic(string code, string path)
        {
            var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(LoadAsset<Mesh>(path)).Build();
            WallTowers.Add(code, graphic);
        }

        public void AddKeepPlateauGraphic(string code, string path, Vector3 offset = default)
        {
            var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(LoadAsset<Mesh>(path)).WithOffset(offset).Build();
            KeepPlateaus.Add(code, graphic);
        }

        private T LoadAsset<T>(string path) where T : Resource
        {
            return GD.Load<T>("res://" + path);
        }
    }
}