using System;

namespace SharpTree.Core.Behaviors
{
    public static class FilesystemBehaviourFactory
    {
        public static IFilesystemBehaviour Create(FilesystemBehaviourType type, string rootVolume = null)
        {
            switch (type)
            {
                case FilesystemBehaviourType.Traverse:
                    return TraverseBehaviour.Instance;

                case FilesystemBehaviourType.SingleVolume:
                    if (!string.IsNullOrWhiteSpace(rootVolume))
                    {
                        return new SingleVolumeBehaviour(rootVolume);
                    }
                    throw new ArgumentException("Invalid FilesystemBehaviour type or missing root volume.");

                default:
                    throw new ArgumentException("Invalid FilesystemBehaviour type or missing root volume.");
            }
        }
    }
}