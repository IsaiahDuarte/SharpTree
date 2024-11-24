using SharpTree.Core.Models;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SharpTree.Core.Services
{
    public static class FileSystemReader
    {
        public static RootNode Read(string path, long minSize = 0, int maxDepth = -1)
        {
            if (!Directory.Exists(path))
            {
                throw new System.ArgumentException($"Path {path} does not exist", nameof(path));
            }

            return (RootNode)ReadRecursive(path, minSize, maxDepth);
        }

        public static INode ReadRemote(string remoteComputer, string sharedFolder, string path, long minSize = 0, int maxDepth = -1)
        {
            string uncPath = Path.Combine($"\\\\{remoteComputer}\\{sharedFolder}", path);
            return Read(uncPath, minSize, maxDepth);
        }

        private static INode ReadRecursive(string path, long minSize, int maxDepth = -1, bool isRoot = true, int currentDepth = 0)
        {
            var directoryInfo = new DirectoryInfo(path);
            var node = new DirectoryNode(directoryInfo.Name);
            long totalsize = isRoot ? 0 : node.Size;

            IEnumerable<FileSystemInfo> entries;
            try
            {
                entries = directoryInfo.EnumerateFileSystemInfos();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading {path}: {ex.Message}");
                return node;
            }

            foreach (var entry in entries)
            {
                if (entry.Attributes.HasFlag(FileAttributes.ReparsePoint))
                {
                    continue;
                }

                try
                {
                    switch (entry)
                    {
                        case FileInfo fileInfo:
                            totalsize += fileInfo.Length;
                            if (fileInfo.Length >= minSize)
                            {
                                node.AddChild(new FileNode(fileInfo.Name, fileInfo.Length));
                            }
                            break;

                        case DirectoryInfo dirInfo:
                            if (maxDepth == -1 || currentDepth < maxDepth)
                            {
                                var childDir = ReadRecursive(dirInfo.FullName, minSize, maxDepth, false, currentDepth + 1);
                                totalsize += childDir.Size;
                                if (childDir.Size > 0)
                                {
                                    node.AddChild(childDir);
                                }
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error processing {entry.FullName}: {ex.Message}");
                }
            }

            node.SortChildren();

            if (isRoot && node.IsDirectory)
            {
                return new RootNode(node.Name, totalsize, node.Children);
            }

            return node;
        }
    }
}