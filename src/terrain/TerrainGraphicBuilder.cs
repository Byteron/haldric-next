using Godot;

public class TerrainGraphicBuilder
{
    TerrainGraphic _graphic;

    public TerrainGraphicBuilder Create()
    {
        _graphic = new TerrainGraphic();
        return this;
    }

    public TerrainGraphicBuilder WithCode(string code)
    {
        _graphic.Code = code;
        return this;
    }

    public TerrainGraphicBuilder WithMesh(Mesh mesh)
    {
        _graphic.Mesh = mesh;
        return this;
    }

    public TerrainGraphic Build()
    {
        return _graphic;
    }
}