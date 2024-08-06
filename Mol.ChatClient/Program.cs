
using Mol.ChatClient;

var client = new ChatHubClient();

Console.Write("id>");
var idInput = Console.ReadLine();
Console.Write("nickname>");
var nicknameInput = Console.ReadLine();

await client.ConnectAsync(int.Parse(idInput), nicknameInput);

while (true)
{
    var input = Console.ReadLine();
    switch (input)
    {
        case null:
            break;
        case "!exit":
            await client.DisposeAsync();
            await client.LeaveAsync();
            return;
        default:
            await client.SendMessageAsync(input);
            break;
    }
}