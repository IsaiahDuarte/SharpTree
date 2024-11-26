using System.Collections.Generic;
using System.Linq; 
using SharpTree.Core.Services;

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
        public long Size { get; set; }
        public bool IsDirectory
        {
            get { return true; }
        }
        public IEnumerable<INode> Children
        {
            get { return _children.AsEnumerable(); }
        }

        public void SortChildren() => _children.Sort((a, b) => b.Size.CompareTo(a.Size));
        public void AddChild(INode child) => _children.Add(child);
        public void Show() => NodeViewer.Show(this);
        public void SaveToJson(string path) => JsonNode.SaveToJson(this, path);
        public INode LoadFromJson(string path) => JsonNode.LoadFromJson(path);
        public int GetFileCount() => IsDirectory ? Children.Sum(child => child.GetFileCount()) : 1;
    }
}