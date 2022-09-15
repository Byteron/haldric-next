using System.Collections.Generic;
using RelEcs;
using Godot;

public class LoadTerrainGraphicDataTrigger
{
}

public class LoadTerrainGraphicDataSystem : ISystem
{
    public World World { get; set; }

    public void Run()
    {
        foreach (var _ in this.Receive<LoadTerrainGraphicDataTrigger>())
        {
            var terrainData = this.GetElement<TerrainData>();

            var graphicData = new TerrainGraphicData();
            this.AddOrReplaceElement(graphicData);

            var terrainScript = new TerrainGraphicScript();
            terrainScript.Load();

            graphicData.Decorations = terrainScript.Decorations;
            graphicData.DirectionalDecorations = terrainScript.DirectionalDecorations;
            graphicData.WaterGraphics = terrainScript.WaterGraphics;
            graphicData.WallSegments = terrainScript.WallSegments;
            graphicData.WallTowers = terrainScript.WallTowers;
            graphicData.OuterCliffs = terrainScript.OuterCliffs;
            graphicData.InnerCliffs = terrainScript.InnerCliffs;
            graphicData.KeepPlateaus = terrainScript.KeepPlateaus;
            graphicData.TerrainTextures = terrainScript.TerrainTextures;
            graphicData.TerrainNormalTextures = terrainScript.TerrainNormalTextures;
            graphicData.TerrainRoughnessTextures = terrainScript.TerrainRoughnessTextures;
            graphicData.DefaultOverlayBaseTerrains = terrainScript.DefaultOverlayBaseTerrains;

            var terrainCount = 0;
            foreach (var (code, entity) in terrainData.TerrainEntities)
            {
                this.On(entity).Add(new TerrainTypeIndex { Value = terrainCount });

                graphicData.TextureArrayIds.Add(code, terrainCount);
                terrainCount += 1;
            }

            graphicData.TextureArray = CreateTextureArray(terrainCount, graphicData, graphicData.TextureArray,
                graphicData.TerrainTextures);

            graphicData.NormalTextureArray = CreateTextureArray(terrainCount, graphicData,
                graphicData.NormalTextureArray,
                graphicData.TerrainNormalTextures);

            graphicData.RoughnessTextureArray = CreateTextureArray(terrainCount, graphicData,
                graphicData.RoughnessTextureArray,
                graphicData.TerrainRoughnessTextures);
        }
    }

    static Texture2DArray CreateTextureArray(int terrainCount, TerrainGraphicData data, Texture2DArray texArray,
        Dictionary<string, Texture2D> textureDict)
    {
        var textures = new Godot.Collections.Array<Image>();
        textures.Resize(terrainCount);

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