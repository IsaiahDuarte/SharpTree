using System.Collections.Generic;
using System.Linq; 
namespace SharpTree.Core.Models
{
    public class DirectoryNode : INode
    {
        private readonly List<INode> _children = new List<INode>();

        public DirectoryNode(string name)
        {
            Name = name;
        }

        public string Name { get; private set; } 
        public long Size
        {
            get
            {
                return _children.Sum(child => child.Size);
            }
        }
        public bool IsDirectory
        {
            get { return true; }
        }
        public IEnumerable<INode> Children
        {
            get { return _children.AsEnumerable(); }
        }

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