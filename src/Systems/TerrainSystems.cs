using System.Collections.Generic;
using Godot;

public static class TerrainSystems
{
    public static void LoadTerrains(World world)
    {
        var terrainData = new TerrainData();
        world.AddOrReplaceElement(terrainData);

        terrainData.Terrains = Loader.LoadJson<Dictionary<string, TerrainInfo>>("res://data/terrain.json");

        foreach (var (code, data) in terrainData.Terrains)
        {
            var entityBuilder = world.Spawn();

            if (data.IsBase) entityBuilder.Add<IsBaseTerrain>();
            else entityBuilder.Add<IsOverlayTerrain>();

            entityBuilder.Add(new TerrainCode { Value = data.Code })
                .Add(new TerrainTypes { List = data.Types })
                .Add(new ElevationOffset { Value = data.ElevationOffset });

            if (data.CanRecruitFrom) entityBuilder.Add<CanRecruitFrom>();
            if (data.CanRecruitTo) entityBuilder.Add<CanRecruitTo>();
            if (data.GivesIncome) entityBuilder.Add<GivesIncome>();
            if (data.IsCapturable) entityBuilder.Add<IsCapturable>();
            if (data.Heals) entityBuilder.Add<Heals>();

            if (!string.IsNullOrEmpty(data.DefaultBase))
            {
                terrainData.DefaultOverlayBaseTerrains[code] = data.DefaultBase;
            }

            terrainData.TerrainEntities.Add(code, entityBuilder.Id());
        }
    }

    public static void LoadTerrainGraphics(World world)
    {
        var terrainData = world.GetElement<TerrainData>();

        var graphicData = new TerrainGraphicData();
        world.AddOrReplaceElement(graphicData);

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

        var terrainCount = 0;
        foreach (var (code, entity) in terrainData.TerrainEntities)
        {
            world.On(entity).Add(new TerrainTypeIndex { Value = terrainCount });

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