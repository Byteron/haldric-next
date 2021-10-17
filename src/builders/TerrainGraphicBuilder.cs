using Godot;

public class TerrainGraphic
{
    public string Code;
    public Mesh Mesh;
    public Vector3 Offset;
}

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

    public TerrainGraphicBuilder WithOffset(Vector3 offset)
    {
        _graphic.Offset = offset;
        return this;
    }

    public TerrainGraphic Build()
    {
        return _graphic;
    }
}