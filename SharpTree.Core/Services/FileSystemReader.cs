using SharpTree.Core.Behaviors;
using SharpTree.Core.Models;
using System.IO;
using System.Reflection;

namespace SharpTree.Core.Services
{
    public static class FileSystemReader
    {
        public static INode ReadRecursive(string path, bool followSymlinks, IFilesystemBehavior fsBehaviour, long minSize, int maxDepth = -1, bool isRoot = true, int currentDepth = 0)
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
                if (!followSymlinks && entry.Attributes.HasFlag(FileAttributes.ReparsePoint))
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
                                var newFsBehaviour = fsBehaviour.GetNextLevel(dirInfo);
                                if (newFsBehaviour == null)
                                    break;
                                
                                var childDir = ReadRecursive(dirInfo.FullName, followSymlinks, newFsBehaviour, minSize, maxDepth, false, currentDepth + 1);
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