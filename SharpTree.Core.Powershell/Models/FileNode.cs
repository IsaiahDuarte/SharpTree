using System.Collections.Generic;
using System.Linq;
using SharpTree.Core.Services;

namespace SharpTree.Core.Models
{
    public class FileNode : INode
    {
        public FileNode(string name, long size)
        {
            Name = name;
            Size = size;
        }

        public string Name { get; private set; }
        public long Size { get; private set; }
        public bool IsDirectory { get { return false; } }
        public IEnumerable<INode> Children => new List<INode>();
        public void Show() => NodeViewer.Show(this);
        public void SaveToJson(string path) => JsonNode.SaveToJson(this, path);
        public INode LoadFromJson(string path) => JsonNode.LoadFromJson(path);
        public int GetFileCount() => IsDirectory ? Children.Sum(child => child.GetFileCount()) : 1;
    }
}