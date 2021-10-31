using System.Collections.Generic;
using Godot;
using Haldric.Wdk;

public class Daytime
{
    public float Angle = 60f;
    public float Energy = 0f;
    public Color Color;

    private List<Alignment> _bonuses;
    private List<Alignment> _maluses;

    public Daytime(float angle, float energy, Color color, List<Alignment> bonuses, List<Alignment> maluses)
    {
        Angle = angle;
        Energy = energy;
        Color = color;
        _bonuses = bonuses;
        _maluses = maluses;
    }

    public float GetDamageModifier(Alignment alignment)
    {
        float mod = 1f;

        if (_bonuses != null && _bonuses.Contains(alignment))
        {
            mod += 0.25f;
        }

        if (_maluses != null && _maluses.Contains(alignment))
        {
            mod -= 0.25f;
        }

        return mod;
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