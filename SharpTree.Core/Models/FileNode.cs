namespace SharpTree.Core.Models
{
    public class FileNode : INode
    {
        public FileNode(string name, long size)
        {
            Name = name;
            Size = size;
        }

        public string Name { get; }
        public long Size { get; }
        public bool IsDirectory => false;
        public IEnumerable<INode> Children => Enumerable.Empty<INode>();
    }
}
