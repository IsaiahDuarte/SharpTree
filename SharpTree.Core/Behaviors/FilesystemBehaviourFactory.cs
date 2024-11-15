namespace SharpTree.Core.Behaviors
{
    public static class FilesystemBehaviourFactory
    {
        public static IFilesystemBehaviour Create(FilesystemBehaviourType type, string? rootVolume = null)
        {
            return type switch
            {
                FilesystemBehaviourType.Traverse => TraverseBehaviour.Instance,
                FilesystemBehaviourType.SingleVolume when !string.IsNullOrWhiteSpace(rootVolume) => new SingleVolumeBehaviour(rootVolume),
                _ => throw new ArgumentException("Invalid FilesystemBehaviour type or missing root volume.")
            };
        }
    }
}
