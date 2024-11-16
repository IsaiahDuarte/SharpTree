using System;
using System.Collections.Generic;
using System.IO;
using SharpTree.Core.Behaviors;
using SharpTree.Core.Models;

namespace SharpTree.Core.Services
{
    public static class FileSystemReader
    {
        public static INode ReadRecursive(string path, bool followSymlinks, IFilesystemBehavior fsBehaviour, long minSize, bool isRoot = true)
        {
            var directoryInfo = new DirectoryInfo(path);
            var node = new DirectoryNode(directoryInfo.Name);
            long totalsize = isRoot ? 0 : node.Size;

            IEnumerable<FileSystemInfo> entries;
            try
            {
                entries = directoryInfo.GetFileSystemInfos();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading {path}: {ex.Message}");
                return node;
            }


            foreach (var entry in entries)
            {
                if (!followSymlinks && (entry.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
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
                            var newFsBehaviour = fsBehaviour.GetNextLevel(dirInfo);
                            if (newFsBehaviour != null)
                            {
                                var childDir = ReadRecursive(dirInfo.FullName, followSymlinks, newFsBehaviour, minSize);
                                node.AddChild(childDir);
                                totalsize += childDir.Size;
                            }
                            break;

                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error processing {entry.FullName}: {ex.Message}");
                }
            }

            if (isRoot && node.IsDirectory && minSize > 0)
            {
                return new RootNode(node.Name, totalsize, node.Children);
            }

            node.SortChildren();
            return node;
        }
    }
}