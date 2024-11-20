using CommandLine;
using Newtonsoft.Json;
using SharpTree.Core.Models;
using SharpTree.Core.Services;
using Terminal.Gui.Trees;
using Terminal.Gui;

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

            INode root = FileSystemReader.Read(
                startPath,
                opts.MinSize,
                opts.MaxDepth);

            if (!(string.IsNullOrEmpty(opts.JsonPath)))
            {
                NodeToJson.SaveToJsonFile(root, opts.JsonPath);
            }

            if (opts.Print)
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                DisplayNode(root, 0);
                Console.WriteLine("Total size: " + root.Size);
            }

            if (opts.Display)
            {
                NodeViewer.Show(root);
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