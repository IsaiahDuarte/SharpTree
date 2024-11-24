using SharpTree.Core.Services;

namespace SharpTree.Core.Models
{
    public interface INode
    {
        string Name { get; }
        long Size { get; }
        bool IsDirectory { get; }
        IEnumerable<INode> Children { get; }

        public void Show() => NodeViewer.Show(this);
        public void SaveToJson(string path) => JsonNode.SaveToJson(this, path);
        public INode? LoadFromJson(string path) => JsonNode.LoadFromJson(path);

        public int GetFileCount() => IsDirectory ? Children.Sum(child => child.GetFileCount()) : 1;
    }

}
