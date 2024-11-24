using SharpTree.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpTree.Core.Models
{
    public class RootNode : INode
    {
        public RootNode(string name, long size, IEnumerable<INode> children)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Name cannot be null or empty");
            }

            Name = name;
            Size = size;
            Children = new List<INode>(children);
        }

        public string Name { get; }
        public long Size { get; }
        public bool IsDirectory => true;

        public IReadOnlyCollection<INode> Children { get; }
        IEnumerable<INode> INode.Children => Children;
        public void Show() => NodeViewer.Show(this);
        public void SaveToJson(string path) => JsonNode.SaveToJson(this, path);
        public INode LoadFromJson(string path) => JsonNode.LoadFromJson(path);
        public int GetFileCount() => IsDirectory ? Children.Sum(child => child.GetFileCount()) : 1;
    }
}