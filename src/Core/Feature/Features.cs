using System;
using System.Collections.Generic;
using System.Linq;
using RelEcs;
using Godot;

public class Features
{
    readonly World _world;
        
    readonly Dictionary<Type, int> _indices = new();
    Feature[] _features = Array.Empty<Feature>();
    bool[] _enabledFeatures = Array.Empty<bool>();

    public Features(World world)
    {
        _world = world;
    }

    public void Init()
    {
        foreach (var feature in _features)
        {
            feature.Init();
        }
        
        foreach (var feature in _features)
        {
            feature.InitSystems.Run(_world);
        }
    }
    
    public void InitFeature<T>() where T : Feature, new()
    {
        var type = typeof(T);
        
        if (_indices.ContainsKey(type))
        {
            GD.PushWarning($"Feature of Type {type.Name} already initialised.");
            return;
        }

        _indices.Add(type, _indices.Count);
        Array.Resize(ref _enabledFeatures, _indices.Count);
        
        _features = _features.ToList().Append(new T()).ToArray();
    }
    
    public void EnableFeature<T>() where T : Feature, new()
    {
        var type = typeof(T);

        if (!_indices.TryGetValue(type, out var index))
        {
            GD.PushError($"Feature of Type {type.Name} not initialised.");
            return;
        }

        ref var enabled = ref _enabledFeatures[index];
        
        if (enabled)
        {
            GD.PushWarning($"Feature of Type {type.Name} already enabled.");
            return;
        }

        var feature = _features[index];
        feature.EnableSystems.Run(_world);
        
        enabled = true;
    }

    public void DisableFeature<T>() where T : Feature
    {
        var type = typeof(T);

        if (!_indices.TryGetValue(type, out var index))
        {
            GD.PushError($"Feature of Type {type.Name} not initialised.");
            return;
        }

        ref var enabled = ref _enabledFeatures[index];
        
        if (!enabled)
        {
            GD.PushWarning($"Feature of Type {type.Name} already disabled.");
            return;
        }
        
        var feature = _features[index];
        feature.DisableSystems.Run(_world);

        enabled = false;
    }

    public void Update()
    {
        for (var i = 0; i < _indices.Count; i++)
        {
            if (!_enabledFeatures[i]) continue;
            _features[i].UpdateSystems.Run(_world);
        }

        _world.Tick();
    }
}