namespace SharpTree.Core.Models
{
    public class DirectoryNode : INode
    {
        private readonly List<INode> _children = new();

        public DirectoryNode(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public long Size => _children.Sum(child => child.Size);
        public bool IsDirectory => true;
        public IEnumerable<INode> Children => _children;

        public void SortChildren()
        {
            _children.Sort((a, b) => b.Size.CompareTo(a.Size));
        }

        public void AddChild(INode child)
        {
            _children.Add(child);
        }
    }
}
