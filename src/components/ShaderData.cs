using Godot;

public class ShaderData
{
    int _width;
    int _height;

    Image _image;
    ImageTexture _texture;
    Color[] _data;

    private Material terrainMaterial = GD.Load<Material>("res://assets/graphics/materials/terrain_material.tres");

    public ShaderData(int width, int height)
    {
        _width = width;
        _height = height;

        if (_image != null)
        {
            _image.Resize(width, height);
        }
        else
        {
            _image = new Image();
            _image.Create(width, height, false, Image.Format.Rgba8);
            _texture = new ImageTexture();
            _texture.CreateFromImage(_image);

            terrainMaterial.Set("shader_param/cell_texture", _texture);
        }

        Vector2 uv = new Vector2(1f / width, 1f / height);
        terrainMaterial.Set("shader_param/texel_size", uv);


        if (_data == null || _data.Length != width * height)
        {
            _data = new Color[width * height];
        }
        else
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = new Color(0f, 0f, 0f, 0f);
            }
        }
    }

    public void UpdateTerrain(int x, int z, int terrainTypeIndex)
    {
        int index = z * _width + x;
        _data[index].g8 = terrainTypeIndex;
        // GD.Print(string.Format("Terrain: Updated {0}, {1} to {2}", x, z, _data[index].g8));
    }

    public void UpdateVisibility(int x, int z, bool isVisible)
    {
        int index = z * _width + x;
        _data[index].a8 = isVisible ? 255 : 0;
        // GD.Print(string.Format("Visibility: Updated {0}, {1} to {2}", x, z, _data[index].a8));
    }

    public void ResetVisibility()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            _data[i].a8 = 255;
        }
    }

    public Texture GetTexture()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            int x = i % _width;
            int z = i / _height;

            var color = _data[i];
            _image.SetPixel(x, z, color);
        }

        _texture.CreateFromImage(_image);
        return _texture;
    }

    public void Apply()
    {
        terrainMaterial.Set("shader_param/cell_texture", GetTexture());
    }

}