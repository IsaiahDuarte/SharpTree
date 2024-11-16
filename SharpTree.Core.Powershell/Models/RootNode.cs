using System;
using System.Collections.Generic;

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
    }
}