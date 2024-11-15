using System.Collections.Generic;

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
    }
}