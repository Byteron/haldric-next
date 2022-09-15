using RelEcs;

public static class FeatureExtensions
{
    public static void Enable<T>(this ISystem system) where T : Feature
    {
        system.GetElement<Features>().EnableFeature<T>();
    }
    
    public static void Disable<T>(this ISystem system) where T : Feature
    {
        system.GetElement<Features>().DisableFeature<T>();
    }
}
