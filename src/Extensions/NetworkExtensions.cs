using Nakama;
using Nakama.TinyJson;
using RelEcs;

public static class NetworkSystemExtensions
{
    public static void SendNetworked<T>(this ISystem system, NetworkOperation operation, T message) where T : class
    {
        system.Send(message);

        if (!system.TryGetElement<ISocket>(out var socket)) return;
        if (!system.TryGetElement<IMatch>(out var match)) return;

        var json = message.ToJson();

        socket.SendMatchStateAsync(match.Id, (int)operation, json);
    }
}
