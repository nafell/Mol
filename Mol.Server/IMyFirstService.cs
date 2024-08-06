using MagicOnion;

namespace Mol.Server;

public interface IMyFirstService : IService<IMyFirstService>
{
    UnaryResult<int> AddAsync(int x, int y);
}