namespace SharpTree.Core.Behaviors
{
    public static class FilesystemBehaviorsFactory
    {
        public static IFilesystemBehavior Create(FilesystemBehaviorType type, string? rootVolume = null)
        {
            return type switch
            {
                FilesystemBehaviorType.Traverse => TraverseBehavior.Instance,
                FilesystemBehaviorType.SingleVolume when !string.IsNullOrWhiteSpace(rootVolume) => new SingleVolumeBehavior(rootVolume),
                _ => throw new ArgumentException("Invalid FilesystemBehaviour typ e or missing root volume.")
            };
        }
    }
}
