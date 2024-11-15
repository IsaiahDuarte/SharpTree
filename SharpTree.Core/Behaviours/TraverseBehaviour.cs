namespace SharpTree.Core.Behaviors
{
    public class TraverseBehaviour : IFilesystemBehaviour
    {
        public static TraverseBehaviour Instance { get; } = new TraverseBehaviour();

        private TraverseBehaviour() { }

        public IFilesystemBehaviour? GetNextLevel(DirectoryInfo directory) => this;
    }
}
