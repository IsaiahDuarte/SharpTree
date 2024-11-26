using SharpTree.Core.Models;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SharpTree.Core.Services
{
    public static class FileSystemReader
    {
        public static INode Read(string path, long minSize = 0, int maxDepth = -1, bool verbose = false)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException($"Path {path} does not exist", nameof(path));

            var directoryInfo = new DirectoryInfo(path);
            var node = new DirectoryNode(directoryInfo.Name);
            long totalSize = 0;

            FileSystemInfo[] entries;
            try
            {
                entries = directoryInfo.GetFileSystemInfos();
            }
            catch (Exception ex)
            {
                if (verbose)
                    Console.Error.WriteLine($"Error reading {path}: {ex.Message}");
                node.Size = 0;
                return node;
            }

            var syncObj = new object();

            Parallel.ForEach(entries, entry =>
            {
                ProcessEntry(entry, node, minSize, maxDepth, verbose, ref totalSize, syncObj, 0);
            });

            node.Size = totalSize;
            node.SortChildren();

            if (node.IsDirectory)
            {
                if (verbose)
                    Console.WriteLine($"Returning RootNode: {node.Name}");
                return new RootNode(node.Name, totalSize, node.Children);
            }

            if (verbose)
                Console.WriteLine($"Returning DirectoryNode: {node.Name}");
            return node;
        }

        private static INode ReadRecursive(string path, long minSize, int maxDepth, bool isRoot, int currentDepth, bool verbose)
        {
            var directoryInfo = new DirectoryInfo(path);
            var node = new DirectoryNode(directoryInfo.Name);
            long totalSize = 0;

            FileSystemInfo[] entries;
            try
            {
                entries = directoryInfo.GetFileSystemInfos();
            }
            catch (Exception ex)
            {
                if (verbose)
                    Console.Error.WriteLine($"Error reading {path}: {ex.Message}");
                node.Size = 0;
                return node;
            }

            foreach (var entry in entries)
            {
                ProcessEntry(entry, node, minSize, maxDepth, verbose, ref totalSize, null, currentDepth);
            }

            node.Size = totalSize;
            node.SortChildren();

            if (isRoot && node.IsDirectory)
            {
                if (verbose)
                    Console.WriteLine($"Returning RootNode: {node.Name}");
                return new RootNode(node.Name, totalSize, node.Children);
            }

            if (verbose)
                Console.WriteLine($"Returning DirectoryNode: {node.Name}");
            return node;
        }

        private static void ProcessEntry(FileSystemInfo entry, DirectoryNode node, long minSize, int maxDepth, bool verbose, ref long totalSize, object syncObj, int currentDepth)
        {
            try
            {
                if ((entry.Attributes & FileAttributes.ReparsePoint) != 0)
                {
                    if (verbose)
                        Console.WriteLine($"Skipping reparse point: {entry.FullName}");
                    return;
                }

                if (entry is FileInfo fileInfo)
                {
                    if (verbose)
                        Console.WriteLine($"Processing file: {fileInfo.FullName}");
                    if (fileInfo.Length >= minSize)
                    {
                        if (syncObj != null)
                        {
                            lock (syncObj)
                            {
                                totalSize += fileInfo.Length;
                                node.AddChild(new FileNode(fileInfo.Name, fileInfo.Length));
                            }
                        }
                        else
                        {
                            totalSize += fileInfo.Length;
                            node.AddChild(new FileNode(fileInfo.Name, fileInfo.Length));
                        }
                    }
                }
                else if (entry is DirectoryInfo dirInfo)
                {
                    if (verbose)
                        Console.WriteLine($"Processing directory: {dirInfo.FullName}");
                    if (maxDepth == -1 || currentDepth < maxDepth)
                    {
                        var childDir = ReadRecursive(
                            dirInfo.FullName,
                            minSize,
                            maxDepth,
                            isRoot: false,
                            currentDepth: currentDepth + 1,
                            verbose);

                        if (childDir.Size > 0)
                        {
                            if (syncObj != null)
                            {
                                lock (syncObj)
                                {
                                    totalSize += childDir.Size;
                                    node.AddChild(childDir);
                                }
                            }
                            else
                            {
                                totalSize += childDir.Size;
                                node.AddChild(childDir);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (verbose)
                    Console.Error.WriteLine($"Error processing {entry.FullName}: {ex.Message}");
            }
        }
    }
}