using System.Collections.Generic;
using RelEcs;
using Godot;

public class LoadTerrainDataTrigger
{
}

public class LoadTerrainDataSystem : ISystem
{
    public World World { get; set; }

    public void Run()
    {
        foreach (var _ in this.Receive<LoadTerrainDataTrigger>())
        {
            var data = new TerrainData();
            this.AddOrReplaceElement(data);

            var terrainScript = new TerrainScript();
            terrainScript.Load();

            data.TerrainDicts = terrainScript.TerrainDicts;

            foreach (var pair in data.TerrainDicts)
            {
                data.Terrains.Add(pair.Key, TerrainFactory.CreateFromDict(World, pair.Value));
            }

            data.Decorations = terrainScript.Decorations;
            data.DirectionalDecorations = terrainScript.DirectionalDecorations;
            data.WaterGraphics = terrainScript.WaterGraphics;
            data.WallSegments = terrainScript.WallSegments;
            data.WallTowers = terrainScript.WallTowers;
            data.OuterCliffs = terrainScript.OuterCliffs;
            data.InnerCliffs = terrainScript.InnerCliffs;
            data.KeepPlateaus = terrainScript.KeepPlateaus;
            data.TerrainTextures = terrainScript.TerrainTextures;
            data.TerrainNormalTextures = terrainScript.TerrainNormalTextures;
            data.TerrainRoughnessTextures = terrainScript.TerrainRoughnessTextures;
            data.DefaultOverlayBaseTerrains = terrainScript.DefaultOverlayBaseTerrains;

            var index = 0;
            foreach (var (code, entity) in data.Terrains)
            {
                this.On(entity).Add(new TerrainTypeIndex { Value = index });

                data.TextureArrayIds.Add(code, index);
                index += 1;
            }

            data.TextureArray = CreateTextureArray(data, data.TextureArray, data.TerrainTextures);
            data.NormalTextureArray = CreateTextureArray(data, data.NormalTextureArray, data.TerrainNormalTextures);
            data.RoughnessTextureArray = CreateTextureArray(data, data.RoughnessTextureArray, data.TerrainRoughnessTextures);
        }
    }

    static Texture2DArray CreateTextureArray(TerrainData data, Texture2DArray texArray,
        Dictionary<string, Texture2D> textureDict)
    {
        var textures = new Godot.Collections.Array<Image>();
        textures.Resize(data.Terrains.Count);

        for (var i = 0; i < textures.Count; i++)
        {
            textures[i] = textureDict["Gg"].GetImage();
        }

        foreach (var (code, texture) in textureDict)
        {
            var index = data.TextureArrayIds[code];

            var image = texture.GetImage();
            textures[index] = image;
        }

        texArray._Images = textures;

        return texArray;
    }
}