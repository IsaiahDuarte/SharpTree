namespace SharpTree.Core.Behaviors
{
    public class SingleVolumeBehaviour : IFilesystemBehaviour
    {
        public string RootVolume { get; }

        public SingleVolumeBehaviour(string rootVolume)
        {
            if (string.IsNullOrWhiteSpace(rootVolume))
                throw new ArgumentException("Root volume must be a valid non-empty path.", nameof(rootVolume));

            RootVolume = Path.GetPathRoot(rootVolume)
                         ?? throw new ArgumentException("Invalid root volume path.", nameof(rootVolume));
        }

        public IFilesystemBehaviour? GetNextLevel(DirectoryInfo directory)
        {
            string currentVolume = Path.GetPathRoot(directory.FullName) ?? string.Empty;
            return string.Equals(RootVolume, currentVolume, StringComparison.OrdinalIgnoreCase) ? this : null;
        }
    }

}
