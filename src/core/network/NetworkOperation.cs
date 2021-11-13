using System;

public enum NetworkOperation
{
    FactionSelected,
}

[Serializable]
public struct FactionSelectedMessage
{
    public int Side { get; set; }
    public int Index { get; set; }
}