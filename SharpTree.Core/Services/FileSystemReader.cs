using SharpTree.Core.Behaviors;
using SharpTree.Core.Models;

namespace SharpTree.Core.Services
{
    public static class FileSystemReader
    {
        public static INode ReadRecursive(string path, bool followSymlinks, IFilesystemBehavior fsBehaviour)
        {
            var directoryInfo = new DirectoryInfo(path);
            var node = new DirectoryNode(directoryInfo.Name);

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
                            node.AddChild(new FileNode(fileInfo.Name, fileInfo.Length));
                            break;

                        case DirectoryInfo dirInfo:
                            var newFsBehaviour = fsBehaviour.GetNextLevel(dirInfo);
                            if (newFsBehaviour != null)
                            {
                                var childDir = ReadRecursive(dirInfo.FullName, followSymlinks, newFsBehaviour);
                                node.AddChild(childDir);
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
            return node;
        }
    }
}