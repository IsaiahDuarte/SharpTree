using System;

namespace SharpTree.Core.Behaviors
{
    public static class FilesystemBehaviorsFactory
    {
        public static IFilesystemBehavior Create(FilesystemBehaviorType type, string rootVolume = null)
        {
            switch (type)
            {
                case FilesystemBehaviorType.Traverse:
                    return TraverseBehaviors.Instance;

                case FilesystemBehaviorType.SingleVolume:
                    if (!string.IsNullOrWhiteSpace(rootVolume))
                    {
                        return new SingleVolumeBehavior(rootVolume);
                    }
                    throw new ArgumentException("Invalid FilesystemBehaviour type or missing root volume.");

                default:
                    throw new ArgumentException("Invalid FilesystemBehaviour type or missing root volume.");
            }
        }
    }
}