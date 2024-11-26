using SharpTree.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace SharpTree.Core.Models
{
    public interface INode
    {
        string Name { get; }
        long Size { get; set; }
        bool IsDirectory { get; }
        IEnumerable<INode> Children { get; }

        void Show();
        void SaveToJson(string path);
        INode LoadFromJson(string path);
        int GetFileCount();


    }
}