using Godot;
using System;
using Nakama;
using System.Threading.Tasks;

public class ChannelMessageEventArgs : EventArgs
{
    public IApiChannelMessage Message;
}

public partial class Network : Node
{
    public static Network Instance;

    private const string _scheme = "http";
    private const string _host = "49.12.208.4";
    private const int _port = 7350;
    private const string _serverKey = "defaultkey";

    public ISession Session { get; private set; }
    public ISocket Socket { get; private set; }
    public IApiAccount Account { get; private set; }
    private Client _client;

    public override void _Ready()
    {
        Instance = this;    
    }

    public async Task<string> Login(string email, string password, string username)
    {
        _client = new Client(_scheme, _host, _port, _serverKey);

        try
        {
            if (string.IsNullOrEmpty(username))
            {
                Session = await _client.AuthenticateEmailAsync(email, password);
            }
            else
            {
                Session = await _client.AuthenticateEmailAsync(email, password, username);
            }

            GD.Print(Session.AuthToken); // raw JWT token
            GD.Print(Session.RefreshToken); // raw JWT token.
            GD.Print(Session.UserId);
            GD.Print(Session.Username);
            GD.Print("Session has expired: {0}", Session.IsExpired);
            GD.Print("Session expires at: {0}", Session.ExpireTime);

            Account = await _client.GetAccountAsync(Session);
            GD.Print(Account.User.Id);
            GD.Print(Account.User.Username);
            GD.Print(Account.Wallet);

            Socket = Nakama.Socket.From(_client);
            Socket.Connected += () => GD.Print("Connected!");
            Socket.Closed += () => GD.Print("Closed!");
            Socket.ReceivedError += (e) => GD.Print(e);

            await Socket.ConnectAsync(Session);
        }
        catch (Exception e)
        {
            return e.Message;
        }

        return "";
    }
}
