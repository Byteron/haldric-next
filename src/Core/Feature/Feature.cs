using RelEcs;

public abstract class Feature
{
    public readonly SystemGroup InitSystems = new();
    public readonly SystemGroup EnableSystems = new();
    public readonly SystemGroup DisableSystems = new();
    public readonly SystemGroup UpdateSystems = new();

    public abstract void Init();
}