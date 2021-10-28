using System.Collections.Generic;
using Godot;

public class Daytime
{
    public float Angle = 60f;
    public float Energy = 0f;
    public Color Color;

    public Daytime(float angle, float energy, Color color)
    {
        Angle = angle;
        Energy = energy;
        Color = color;
    }
}

public class Schedule
{
    public List<Daytime> _list;

    public int _current = 0;

    public Schedule(List<Daytime> list)
    {
        _list = list;
    }

    public void Next()
    {
        _current = (_current + 1) % _list.Count;
    }

    public Daytime GetCurrentDaytime()
    {
        return _list[_current];
    }
}