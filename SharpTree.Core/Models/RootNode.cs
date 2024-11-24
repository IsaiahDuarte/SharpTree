namespace SharpTree.Core.Models
{
    public class RootNode : INode
    {
        public RootNode(string name, long size, List<INode> children)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            }

            Name = name;
            Size = size;
            Children = children ?? new List<INode>();
        }

        public string Name { get; }
        public long Size { get; set; }
        public bool IsDirectory => true;
        public List<INode> Children { get; set; }
        IEnumerable<INode> INode.Children => Children;
        public int GetFileCount() => IsDirectory ? Children.Sum(child => child.GetFileCount()) : 1;

    }
}
