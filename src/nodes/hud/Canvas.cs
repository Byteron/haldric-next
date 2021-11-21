using Godot;
using System.Collections.Generic;

public partial class Canvas : CanvasLayer
{
    Dictionary<int, CanvasLayer> _canvases = new Dictionary<int, CanvasLayer>();

    public CanvasLayer GetCanvasLayer(int layer)
    {
        if (!_canvases.TryGetValue(layer, out var canvasLayer))
        {
            canvasLayer = new CanvasLayer();
            canvasLayer.Layer = layer;
            _canvases.Add(layer, canvasLayer);
            AddChild(canvasLayer);
        }

        return canvasLayer;
    }
}