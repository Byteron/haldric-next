using System.Diagnostics;
using Godot;

namespace Haldric;

public class TerrainShader
{
    readonly int _width;
    readonly int _height;

    readonly Image _image;
    readonly ImageTexture _texture;
    readonly Color[] _data;

    public TerrainShader(int width, int height)
    {
        _width = width;
        _height = height;

        GD.Print(_width, _height);

        _image = Image.Create(width, height, false, Image.Format.Rgba8);
        _texture = ImageTexture.CreateFromImage(_image);

        _data = new Color[width * height];

        var texelX = 1f / width;
        var texelY = 1f / height;
        var texelSize = new Vector2(texelX, texelY);
        Data.Instance.TerrainMaterial.Set("shader_parameter/texel_size", texelSize);

        ResetVisibility(true);
        ResetLighting(true);

        Apply();
    }

    public void UpdateTerrain(int x, int z, int terrainTypeIndex)
    {
        var index = z * _width + x;
        _data[index].A8 = terrainTypeIndex;
    }

    public void UpdateLighting(int x, int z, bool isLit)
    {
        var index = z * _width + x;
        _data[index].B8 = isLit ? 255 : 0;
    }

    public void UpdateVisibility(int x, int z, bool isVisible)
    {
        var index = z * _width + x;
        _data[index].R8 = isVisible ? 255 : 0;
    }

    public void Apply()
    {
        var texture = GetTexture();
        Data.Instance.TerrainMaterial.Set("shader_parameter/cell_texture", texture);
    }

    void ResetLighting(bool isLit)
    {
        for (var i = 0; i < _data.Length; i++)
        {
            _data[i].B8 = isLit ? 255 : 0;
        }
    }

    void ResetVisibility(bool isVisible)
    {
        for (var i = 0; i < _data.Length; i++)
        {
            _data[i].R8 = isVisible ? 255 : 0;
        }
    }

    ImageTexture GetTexture()
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
}