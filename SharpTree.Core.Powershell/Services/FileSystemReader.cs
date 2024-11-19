using System;
using System.Collections.Generic;
using System.IO;
using SharpTree.Core.Models;

namespace SharpTree.Core.Services
{
    public static class FileSystemReader
    {
        public static INode Read(string path, long minSize = 0, int maxDepth = -1)
        {

            if (!Directory.Exists(path))
            {
                throw new System.ArgumentException($"Path {path} does not exist", nameof(path));
            }

            return ReadRecursive(path, minSize, maxDepth);
        }

        private static INode ReadRecursive(string path, long minSize, int maxDepth = -1, bool isRoot = true, int currentDepth = 0)
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
                if ((entry.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
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
                                node.AddChild(childDir);
                                if (node.Size > 0)
                                {
                                    totalsize += childDir.Size;
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

            if (isRoot && node.IsDirectory && minSize > 0)
            {
                return new RootNode(node.Name, node.Size, node.Children);
            }

            return node;
        }
    }
}