using System.Collections.Generic;
using Godot;
using RelEcs;

public class ScenarioData
{
    public Dictionary<string, PackedScene> Schedules = new();
    public Dictionary<string, MapData> Maps = new();
}