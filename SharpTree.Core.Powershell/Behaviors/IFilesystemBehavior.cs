using System.IO;

namespace SharpTree.Core.Behaviors
{
    public interface IFilesystemBehavior
    {
        IFilesystemBehavior GetNextLevel(DirectoryInfo directory);
    }
}