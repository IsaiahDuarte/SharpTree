using CommandLine;
using SharpTree.Core.Behaviors;
using SharpTree.Core.Models;
using SharpTree.Core.Services;

namespace SharpTree
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunWithOptions);

            return result.Tag == ParserResultType.Parsed ? 0 : 1;
        }

        static void RunWithOptions(Options opts)
        {
            string startPath = opts.StartPath;

            if (!Directory.Exists(startPath))
            {
                Console.Error.WriteLine($"The specified path does not exist: {startPath}");
                return;
            }

            var fsBehaviors = FilesystemBehaviorsFactory.Create(
                FilesystemBehaviorType.SingleVolume,
                startPath);

            INode root = FileSystemReader.ReadRecursive(
                startPath,
                opts.FollowSymlinks,
                fsBehaviors,
                opts.MaxSize);

            if (opts.Print)
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                DisplayNode(root, 0);
                Console.WriteLine("Total size: " + root.Size);
            }

        }

        static void DisplayNode(INode node, int indent)
        {
            string icon = node.IsDirectory ? "🗀" : "🗋";
                
            string indentString = new string(' ', indent * 2);
            Console.WriteLine($"{indentString}{icon} {node.Name} ({node.Size} bytes)");

            if (node.IsDirectory && node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    DisplayNode(child, indent + 1);
                }
            }
        }
    }
}