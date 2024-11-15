using CommandLine;
using System.CommandLine;
using System.Runtime.InteropServices;

namespace SharpTree
{
    public class Options
    {
        [Value(0, MetaName = "startPath", HelpText = "Path to start traversal.", Required = false)]
        public string StartPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        [Option('s', "follow-symlinks", HelpText = "Follow symbolic links.", Required = false, Default = false)]
        public bool FollowSymlinks { get; set; }

    }
}