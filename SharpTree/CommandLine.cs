using CommandLine;
using System.CommandLine;
using System.Runtime.InteropServices;

namespace SharpTree
{
    public class Options
    {
        [Value(0, MetaName = "startPath", HelpText = "Path to start traversal.", Required = false)]
        public string StartPath { get; set; } = Environment.CurrentDirectory;

        public bool FollowSymlinks { get; set; }

        [Option('p', "print", HelpText = "Prints the node tree.", Required = false, Default = false)]
        public bool Print { get; set; }

        [Option('m', "MinSize", HelpText = "Restricts size saved to the node.", Required = false, Default = 0)]
        public long MinSize { get; set; }

        [Option('d', "maxdepth", HelpText = "Restricts how deep the traversal goes, 0 is unlimited", Required = false, Default = -1)]
        public int MaxDepth { get; set; }

        [Option('D', "display", HelpText = "Displays the node tree.", Required = false, Default = false)]
        public bool Display { get; set; }

        [Option('v', "verbose", HelpText = "Changes message level", Required = false, Default = false)]
        public bool Verbose { get; set; }

        [Option('j', "jsonpath", HelpText = "Exports to json given the path", Required = false)]
        public string? JsonPath { get; set; }

        [Option('l', "loadjson", HelpText = "loads a json file and displays node", Required = false)]
        public string? LoadJson { get; set; }
    }
}
