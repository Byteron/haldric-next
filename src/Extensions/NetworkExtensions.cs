using Nakama;
using Nakama.TinyJson;
using RelEcs;

public static class NetworkSystemExtensions
{
    public static void SendNetworked<T>(this World world, NetworkOperation operation, T message) where T : class
    {
        world.Send(message);

        if (!world.TryGetElement<ISocket>(out var socket)) return;
        if (!world.TryGetElement<IMatch>(out var match)) return;

        var json = message.ToJson();

        socket.SendMatchStateAsync(match.Id, (int)operation, json);
    }
}
