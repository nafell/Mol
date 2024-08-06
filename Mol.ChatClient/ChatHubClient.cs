using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using MagicOnion.Client;
using Mol.Server;

namespace Mol.ChatClient;

public class ChatHubClient : IChatHubReceiver
{
    private Dictionary<int, string> users = new Dictionary<int, string>();

    private IChatHub client;
    
    public async ValueTask<string> ConnectAsync(int id, string nickname) 
    {
        
        var handler = new HttpClientHandler();
        var certificate = new X509Certificate2("/home/nafell/source/learncodes/magicOnionLearn/Mol.Server/server.crt");
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
        {
            if (cert.GetCertHashString() == certificate.GetCertHashString())
                return true;
            return false;
        };

    
        // Create HttpClient with the custom handler
        var httpClient = new HttpClient(handler);

        // CreatE GrpcChannelOptions with the HttpClient
        var channelOptions = new GrpcChannelOptions
        {
            HttpClient = httpClient
        };
        var channel = GrpcChannel.ForAddress("https://localhost:5001", channelOptions);
        client = await StreamingHubClient.ConnectAsync<IChatHub, IChatHubReceiver>(channel, this);

        var groupUsers = await client.JoinAsync(id, nickname);
        foreach (var user in groupUsers)
        {
            (this as IChatHubReceiver).OnJoin(user);
        }

        return users[id];
    }
    
    public ValueTask LeaveAsync()
    {
        return client.LeaveAsync();
    }
    
    public ValueTask SendMessageAsync(string message)
    {
        return client.SendMessageAsync(message);
    }
    
    public Task DisposeAsync()
    {
        return client.DisposeAsync();
    }
    
    public Task WaitForDisconnect()
    {
        return client.WaitForDisconnect();
    }
    
    void IChatHubReceiver.OnJoin(User user)
    {
        Console.WriteLine("Join User:" + user.Nickname);
        users[user.Id] = user.Nickname;
    }
    
    void IChatHubReceiver.OnLeave(User user)
    {
        Console.WriteLine("Leave User:" + user.Nickname);
        
        if (users.TryGetValue(user.Id, out var nickname))
        {
            //dosomething
        }
    }
    
    void IChatHubReceiver.OnMessage(User user, string message)
    {
        Console.WriteLine($"[{user.Nickname}] {message}");
    }
}