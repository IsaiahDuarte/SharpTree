using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using SharpTree.Core.Models;

namespace SharpTree.Core.Services
{
    public static class FileSystemReader
    {
        public static INode Read(string path, long minSize = 0, int maxDepth = -1, bool verbose = false)
        {
            if (!Directory.Exists(path))
            {
                throw new System.ArgumentException($"Path {path} does not exist", nameof(path));
            }

            return ReadRecursive(path, minSize, maxDepth, true, 0, verbose);
        }

        private static INode ReadRecursive(string path, long minSize, int maxDepth = -1, bool isRoot = true, int currentDepth = 0, bool verbose = false)
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
                    if(verbose)
                        Console.WriteLine($"Skipping ReparsePoint: {entry.FullName}");
                    continue;
                }

                try
                {
                    switch (entry)
                    {
                        case FileInfo fileInfo:
                            if(verbose)
                                Console.WriteLine($"Processing file: {fileInfo.FullName}");
                            totalsize += fileInfo.Length;
                            if (fileInfo.Length >= minSize)
                            {
                                node.AddChild(new FileNode(fileInfo.Name, fileInfo.Length));
                            }
                            break;

                        case DirectoryInfo dirInfo:
                            if(verbose)
                                Console.WriteLine($"Processing directory: {dirInfo.FullName}");
                            if (maxDepth == -1 || currentDepth < maxDepth)
                            {
                                var childDir = ReadRecursive(dirInfo.FullName, minSize, maxDepth, false, currentDepth + 1, verbose);
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
                if(verbose)
                    Console.WriteLine($"Returning RootNode: {node.Name}");
                return new RootNode(node.Name, totalsize, node.Children);
            }

            if(verbose)
                Console.WriteLine($"Returning DirectoryNode: {node.Name}");
            return node;
        }
    }
}