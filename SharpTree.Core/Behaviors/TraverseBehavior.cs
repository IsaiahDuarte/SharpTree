namespace SharpTree.Core.Behaviors
{
    public class TraverseBehavior : IFilesystemBehavior
    {
        public static TraverseBehavior Instance { get; } = new TraverseBehavior();

        private TraverseBehavior() { }

        public IFilesystemBehavior? GetNextLevel(DirectoryInfo directory) => this;
    }
}
