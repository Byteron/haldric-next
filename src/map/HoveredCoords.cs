using Leopotam.Ecs;

public struct HoveredCoords : IEcsAutoReset<HoveredCoords>
{
    public Coords Coords;

    public void AutoReset(ref HoveredCoords c)
    {
        c.Coords = Coords.FromOffset(0, 0);
    }
}