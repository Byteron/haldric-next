using Godot;

public class ShaderData
{
    readonly int _width;
    readonly int _height;

    readonly Image _image;
    readonly ImageTexture _texture;
    readonly Color[] _data;

    readonly Material _terrainMaterial;

    public ShaderData(int width, int height)
    {
        _terrainMaterial = ResourceLoader.Load<Material>("res://assets/graphics/materials/terrain.tres");

        _width = width;
        _height = height;

        _image = new Image();
        _image.Create(width, height, false, Image.Format.Rgba8);
        _texture = new ImageTexture();
        _texture.CreateFromImage(_image);

        _data = new Color[width * height];

        _terrainMaterial.Set("shader_param/cell_texture", _texture);
        _terrainMaterial.Set("shader_param/texel_size", new Vector2(1f / width, 1f / height));

        ResetVisibility(true);
        ResetLighting(true);
    }

    public void UpdateTerrain(int x, int z, int terrainTypeIndex)
    {
        var index = z * _width + x;
        _data[index].a8 = terrainTypeIndex;
    }

    public void UpdateLighting(int x, int z, bool isLit)
    {
        var index = z * _width + x;

        if (index >= _data.Length || index < 0)
        {
            return;
        }

        _data[index].b8 = isLit ? 255 : 0;
    }

    void ResetLighting(bool isLit)
    {
        for (var i = 0; i < _data.Length; i++)
        {
            _data[i].b8 = isLit ? 255 : 0;
        }
    }

    public void UpdateVisibility(int x, int z, bool isVisible)
    {
        var index = z * _width + x;

        if (index >= _data.Length || index < 0) return;

        _data[index].r8 = isVisible ? 255 : 0;
    }

    void ResetVisibility(bool isVisible)
    {
        for (var i = 0; i < _data.Length; i++)
        {
            _data[i].r8 = isVisible ? 255 : 0;
        }
    }

    Texture GetTexture()
    {
        for (var i = 0; i < _data.Length; i++)
        {
            var x = i % _width;
            var z = i / _height;

            var color = _data[i];
            _image.SetPixel(x, z, color);
        }

        _texture.Update(_image);
        return _texture;
    }

    public void Apply()
    {
        _terrainMaterial.Set("shader_param/cell_texture", GetTexture());
    }
}