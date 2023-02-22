using System.Collections.Generic;
using Godot;

namespace Haldric;

public abstract class TerrainGraphicLoader
{
    public readonly Dictionary<string, Dictionary<string, TerrainGraphic>> Decorations = new();
    public readonly Dictionary<string, Dictionary<string, TerrainGraphic>> DirectionalDecorations = new();
    public readonly Dictionary<string, TerrainGraphic> WaterGraphics = new();
    public readonly Dictionary<string, TerrainGraphic> WallSegments = new();
    public readonly Dictionary<string, Dictionary<string, TerrainGraphic>> OuterCliffs = new();
    public readonly Dictionary<string, Dictionary<string, TerrainGraphic>> InnerCliffs = new();
    public readonly Dictionary<string, TerrainGraphic> WallTowers = new();
    public readonly Dictionary<string, TerrainGraphic> KeepPlateaus = new();
    public readonly Dictionary<string, Texture2D> TerrainTextures = new();
    public readonly Dictionary<string, Texture2D> TerrainNormalTextures = new();
    public readonly Dictionary<string, Texture2D> TerrainRoughnessTextures = new();
    public readonly Dictionary<string, string> DefaultOverlayBaseTerrains = new();

    public abstract void Load();

    protected void MapBaseToOverlay(string overlayCode, string baseCode)
    {
        DefaultOverlayBaseTerrains.Add(overlayCode, baseCode);
    }

    protected void AddTerrainTexture(string code, string path)
    {
        TerrainTextures.Add(code, LoadAsset<Texture2D>(path));
    }

    protected void AddTerrainNormalTexture(string code, string path)
    {
        TerrainNormalTextures.Add(code, LoadAsset<Texture2D>(path));
    }

    protected void AddTerrainRoughnessTexture(string code, string path)
    {
        TerrainRoughnessTextures.Add(code, LoadAsset<Texture2D>(path));
    }

    protected void AddDecorationGraphic(string code, string path, string name = null)
    {
        if (!Decorations.ContainsKey(code))
        {
            Decorations.Add(code, new Dictionary<string, TerrainGraphic>());
        }

        var dict = Decorations[code];

        if (string.IsNullOrEmpty(name))
        {
            var graphic = new TerrainGraphic { Code = code, Mesh = LoadAsset<Mesh>(path) };
            dict.Add(path, graphic);
        }
        else
        {
            if (!dict.ContainsKey(name))
            {
                var graphic = new TerrainGraphic { Code = code, Mesh = LoadAsset<Mesh>(path) };
                dict.Add(name, graphic);
            }
            else
            {
                var graphic = dict[name];
                graphic.AddVariation(LoadAsset<Mesh>(path));
            }
        }
    }

    protected void AddDirectionalDecorationGraphic(string code, string path, string name = null)
    {
        if (!DirectionalDecorations.ContainsKey(code))
        {
            DirectionalDecorations.Add(code, new Dictionary<string, TerrainGraphic>());
        }

        var dict = DirectionalDecorations[code];

        if (string.IsNullOrEmpty(name))
        {
            var graphic = new TerrainGraphic { Code = code, Mesh = LoadAsset<Mesh>(path) };
            dict.Add(path, graphic);
        }
        else
        {
            if (!dict.ContainsKey(name))
            {
                var graphic = new TerrainGraphic { Code = code, Mesh = LoadAsset<Mesh>(path) };
                dict.Add(name, graphic);
            }
            else
            {
                var graphic = dict[name];
                graphic.AddVariation(LoadAsset<Mesh>(path));
            }
        }
    }

    protected void AddWaterGraphic(string code, string path)
    {
        var graphic = new TerrainGraphic { Code = code, Mesh = LoadAsset<Mesh>(path) };
        WaterGraphics.Add(code, graphic);
    }

    protected void AddWallSegmentGraphic(string code, string path)
    {
        var graphic = new TerrainGraphic { Code = code, Mesh = LoadAsset<Mesh>(path) };
        WallSegments.Add(code, graphic);
    }

    protected void AddOuterCliffGraphic(string code, string path, string materialPath = "", string name = "")
    {
        if (!OuterCliffs.ContainsKey(code))
        {
            OuterCliffs.Add(code, new Dictionary<string, TerrainGraphic>());
        }

        var dict = OuterCliffs[code];

        if (string.IsNullOrEmpty(name))
        {
            var graphic = new TerrainGraphic { Code = code, Mesh = LoadAsset<Mesh>(path) };

            if (!string.IsNullOrEmpty(materialPath))
            {
                graphic.Mesh.SurfaceSetMaterial(0, LoadAsset<Material>(materialPath));
            }

            dict.Add(path, graphic);
        }
        else
        {
            if (!dict.ContainsKey(name))
            {
                var graphic = new TerrainGraphic { Code = code, Mesh = LoadAsset<Mesh>(path) };

                if (!string.IsNullOrEmpty(materialPath))
                {
                    graphic.Mesh.SurfaceSetMaterial(0, LoadAsset<Material>(materialPath));
                }

                dict.Add(name, graphic);
            }
            else
            {
                var graphic = dict[name];
                var mesh = LoadAsset<Mesh>(path);

                if (!string.IsNullOrEmpty(materialPath))
                {
                    mesh.SurfaceSetMaterial(0, LoadAsset<Material>(materialPath));
                }

                graphic.AddVariation(mesh);
            }
        }
    }

    protected void AddInnerCliffGraphic(string code, string path, string materialPath = "", string name = "")
    {
        if (!InnerCliffs.ContainsKey(code))
        {
            InnerCliffs.Add(code, new Dictionary<string, TerrainGraphic>());
        }

        var dict = InnerCliffs[code];

        if (string.IsNullOrEmpty(name))
        {
            var graphic = new TerrainGraphic { Code = code, Mesh = LoadAsset<Mesh>(path) };

            if (!string.IsNullOrEmpty(materialPath))
            {
                graphic.Mesh.SurfaceSetMaterial(0, LoadAsset<Material>(materialPath));
            }

            dict.Add(path, graphic);
        }
        else
        {
            if (!dict.ContainsKey(name))
            {
                var graphic = new TerrainGraphic { Code = code, Mesh = LoadAsset<Mesh>(path) };

                if (!string.IsNullOrEmpty(materialPath))
                {
                    graphic.Mesh.SurfaceSetMaterial(0, LoadAsset<Material>(materialPath));
                }

                dict.Add(name, graphic);
            }
            else
            {
                var graphic = dict[name];
                var mesh = LoadAsset<Mesh>(path);

                if (!string.IsNullOrEmpty(materialPath))
                {
                    mesh.SurfaceSetMaterial(0, LoadAsset<Material>(materialPath));
                }

                graphic.AddVariation(mesh);
            }
        }
    }

    protected void AddWallTowerGraphic(string code, string path)
    {
        var graphic = new TerrainGraphic { Code = code, Mesh = LoadAsset<Mesh>(path) };
        WallTowers.Add(code, graphic);
    }

    public void AddKeepPlateauGraphic(string code, string path, Vector3 offset = default)
    {
        var graphic = new TerrainGraphic { Code = code, Mesh = LoadAsset<Mesh>(path), Offset = offset };
        KeepPlateaus.Add(code, graphic);
    }

    static T LoadAsset<T>(string path) where T : Resource
    {
        return GD.Load<T>("res://" + path);
    }
}