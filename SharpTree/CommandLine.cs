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

        [Option('p', "print", HelpText = "Prints the node tree.", Required = false, Default = false)]
        public bool Print { get; set; }

        [Option('m', "maxsize", HelpText = "Restricts size saved to the node.", Required = false, Default = 0)]
        public long MaxSize { get; set; }
    }
}