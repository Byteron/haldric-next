using RelEcs;
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

public class LoginEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((LoginEvent e) =>
        {
            var client = commands.GetElement<Client>();
            Login(commands, client, e.Username, e.Email, e.Password);
        });
    }

    async void Login(Commands commands, Client client, string username, string email, string password)
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

            commands.AddElement(session);
            commands.AddElement(account);
            commands.AddElement(socket);

            commands.GetElement<GameStateController>().ChangeState(new LobbyState());
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
        }
    }
}