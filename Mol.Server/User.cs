using MessagePack;

namespace Mol.Server;


[MessagePackObject]
public class User
{
    [Key(0)]
    public int Id { get; set; }
    [Key(1)]
    public string Nickname { get; set; }
}
