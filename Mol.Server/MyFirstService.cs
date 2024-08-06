
namespace Mol.Server;
using MagicOnion;
using MagicOnion.Server;

public class MyFirstService : ServiceBase<IMyFirstService>, IMyFirstService
{
    public async UnaryResult<int> AddAsync(int x, int y)
    {
        Console.WriteLine($"Recieved:{x}, {y}");
        return x+y;
    }
}