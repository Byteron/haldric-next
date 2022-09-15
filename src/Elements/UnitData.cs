using System.Collections.Generic;
using Godot;
using RelEcs;

public class UnitData
{
    static readonly Color[] SideColors =
    {
        new("FF0000"),
        new("0000FF"),
        new("00FF00"),
        new("FFFF00"),
        new("00FFFF"),
        new("FF00FF"),
        new("000000"),
        new("FFFFFF"),
    };

    public Dictionary<string, PackedScene> Units = new();
    public Dictionary<string, FactionData> Factions = new();
}