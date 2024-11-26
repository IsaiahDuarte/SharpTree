namespace SharpTree.Core.Models
{
    public class FileNode : INode
    {
        public FileNode(string name, long size)
        {
            Name = name;
            Size = size;
            Children = new List<INode>();
        }

        public string Name { get; }
        public long Size { get; set; }
        public bool IsDirectory => false;

        public List<INode> Children { get; set; }
        IEnumerable<INode> INode.Children => Children;
    }
}
