using Nakama;
using Nakama.TinyJson;
using RelEcs;

public static class NetworkSystems
{
    public static void SendNetworked<T>(World world, NetworkOperation operation, T message) where T : class
    {
        world.Send(message);

        if (!world.TryGetElement<ISocket>(out var socket)) return;
        if (!world.TryGetElement<IMatch>(out var match)) return;

        var json = message.ToJson();

        socket.SendMatchStateAsync(match.Id, (int)operation, json);
    }
    
    public static void EnqueueNetworked(World world, NetworkOperation operation, ICommand command)
    {
        var commander = world.GetElement<Commander>();
        commander.Enqueue(command);
        
        if (!world.TryGetElement<ISocket>(out var socket)) return;
        if (!world.TryGetElement<IMatch>(out var match)) return;

        var json = command.ToJson();

        socket.SendMatchStateAsync(match.Id, (int)operation, json);
    }
}
