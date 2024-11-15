namespace SharpTree.Core.Behaviors
{
    public interface IFilesystemBehaviour
    {
        IFilesystemBehaviour? GetNextLevel(DirectoryInfo directory);
    }
}
