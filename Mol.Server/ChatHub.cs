using MagicOnion;
using MagicOnion.Server.Hubs;
using MessagePack;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Mol.Server;

public interface IChatHub : IStreamingHub<IChatHub, IChatHubReceiver>
{
    ValueTask<User[]> JoinAsync(int userId, string nickname);
    ValueTask LeaveAsync();
    ValueTask SendMessageAsync(string message);
}

public interface IChatHubReceiver
{
    void OnJoin(User user);
    void OnLeave(User user);
    void OnMessage(User user, string message);
}

public class ChatHub : StreamingHubBase<IChatHub, IChatHubReceiver>, IChatHub
{
    private IGroup group;
    private User self;
    private IInMemoryStorage<User> storage;

    public async ValueTask<User[]> JoinAsync(int userId, string nickname)
    {
        self = new User() {Id = userId, Nickname = nickname};
        (group, storage) = await Group.AddAsync("group1", self);
        
        
        //broadcast join event to connected clients.
        Broadcast(group).OnJoin(self);

        Console.WriteLine($"User {self.Id}:{self.Nickname} has joined group {group.GroupName}.");
        return storage.AllValues.ToArray();
    }
    
    public async ValueTask LeaveAsync()
    {
        await group.RemoveAsync(this.Context);
        Broadcast(group).OnLeave(self);
        Console.WriteLine($"User {self.Id}:{self.Nickname} has left group {group.GroupName}.");
    }
    
    public async ValueTask SendMessageAsync(string message) 
    {
        Broadcast(group).OnMessage(self, message);
        Console.WriteLine($"[{self.Id}:{self.Nickname}] {message}");
    }
    
    protected override ValueTask OnDisconnected()
    {
        Console.WriteLine($"Lost connection with user {self.Id}:{self.Nickname}.");
        return ValueTask.CompletedTask;
    }

}