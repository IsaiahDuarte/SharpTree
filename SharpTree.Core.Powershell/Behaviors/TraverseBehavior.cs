using System.IO;

namespace SharpTree.Core.Behaviors
{
    public class TraverseBehaviors : IFilesystemBehavior
    {
        public static TraverseBehaviors Instance { get; } = new TraverseBehaviors();

        private TraverseBehaviors() { }

        public IFilesystemBehavior GetNextLevel(DirectoryInfo directory)
        {
            return this;
        }
    }
}