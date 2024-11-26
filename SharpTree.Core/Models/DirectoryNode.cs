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
        public long Size { get; set; }
        public bool IsDirectory => true;

        public List<INode> Children
        {
            get => _children;
            set
            {
                _children.Clear();
                if (value != null)
                {
                    _children.AddRange(value);
                }
            }
        }

        IEnumerable<INode> INode.Children => Children;

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
