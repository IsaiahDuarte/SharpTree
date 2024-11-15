namespace SharpTree.Core.Models
{
    public interface INode
    {
        string Name { get; }
        long Size { get; }
        bool IsDirectory { get; }
        IEnumerable<INode> Children { get; }
    }
}
