using Bitron.Ecs;
using Nakama;
using Godot;
using System;

public struct LoginEvent
{
    public string Email;
    public string Password;
    public string Username;

    public LoginEvent(string email, string password, string username)
    {
        Email = email;
        Password = password;
        Username = username;
    }
}

public class LoginEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        world.ForEach((ref LoginEvent e) =>
        {
            var client = world.GetResource<Client>();
            Login(world, client, e.Username, e.Email, e.Password);
        });
    }

    private async void Login(EcsWorld world, Client client, string username, string email, string password)
    {
        ISession session;

        try
        {
            if (string.IsNullOrEmpty(username))
            {
                session = await client.AuthenticateEmailAsync(email, password);
            }
            else
            {
                session = await client.AuthenticateEmailAsync(email, password, username);
            }

            IApiAccount account = await client.GetAccountAsync(session); ;
            ISocket socket = Nakama.Socket.From(client);

            socket.Connected += () => GD.Print("Connected!");
            socket.Closed += () => GD.Print("Closed!");
            socket.ReceivedError += (e) => GD.Print(e);
            await socket.ConnectAsync(session);

            world.AddResource(session);
            world.AddResource(account);
            world.AddResource(socket);

            world.GetResource<GameStateController>().ChangeState(new LobbyState(world));
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
        }
    }
}