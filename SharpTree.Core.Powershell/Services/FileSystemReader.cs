using SharpTree.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            var processingNode = new DirectoryNode(directoryInfo.Name);
            long totalSize = 0;
            FileSystemInfo[] entries;
            try
            {
                entries = directoryInfo.GetFileSystemInfos();
            }
            catch (Exception ex)
            {
                if (verbose)
                    Console.Error.WriteLine($"Error reading root {path}: {ex.Message}");
                return new RootNode(directoryInfo.Name, 0, new List<INode>());
            }

            var syncObj = new object();
            Parallel.ForEach(entries, entry =>
            {
                ProcessEntry(entry, processingNode, minSize, maxDepth, verbose, ref totalSize, syncObj, 0);
            });

            processingNode.Size = totalSize;
            processingNode.SortChildren();
            if (verbose)
                Console.WriteLine($"Processed Root: {processingNode.Name} with Size: {processingNode.Size}");

            return new RootNode(processingNode.Name, processingNode.Size, processingNode.Children);
        }

        private static DirectoryNode ReadRecursive(string path, long minSize, int maxDepth, int currentDepth, bool verbose)
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
            if (verbose)
                Console.WriteLine($"Returning DirectoryNode: {node.Name} with Size: {node.Size}");
            return node;
        }

        private static void ProcessEntry(FileSystemInfo entry,
                                         DirectoryNode node,
                                         long minSize,
                                         int maxDepth,
                                         bool verbose,
                                         ref long totalSize,
                                         object syncObj,
                                         int currentDepth)
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

                    long currentFileSize = 0;
                    try
                    {
                        currentFileSize = fileInfo.Length;
                    }
                    catch (FileNotFoundException fnfEx)
                    {
                        if (verbose)
                            Console.Error.WriteLine($"Error accessing file {fileInfo.FullName}: {fnfEx.Message}");
                        return;
                    }
                    catch (IOException ioEx)
                    {
                        if (verbose)
                            Console.Error.WriteLine($"IO Error accessing file {fileInfo.FullName}: {ioEx.Message}");
                        return;
                    }

                    if (syncObj != null)
                    {
                        lock (syncObj)
                        {
                            totalSize += currentFileSize;
                            if (currentFileSize >= minSize)
                                node.AddChild(new FileNode(fileInfo.Name, currentFileSize));
                        }
                    }
                    else
                    {
                        totalSize += currentFileSize;
                        if (currentFileSize >= minSize)
                            node.AddChild(new FileNode(fileInfo.Name, currentFileSize));
                    }
                }
                else if (entry is DirectoryInfo dirInfo)
                {
                    if (verbose)
                        Console.WriteLine($"Processing directory: {dirInfo.FullName}");

                    if (maxDepth == -1 || currentDepth < maxDepth)
                    {
                        var childDirNode = ReadRecursive(dirInfo.FullName, minSize, maxDepth, currentDepth + 1, verbose);
                        long childDirSize = childDirNode.Size;
                        if (childDirSize > 0 || (childDirNode.Children != null && childDirNode.Children.Any()))
                        {
                            if (syncObj != null)
                            {
                                lock (syncObj)
                                {
                                    totalSize += childDirSize;
                                    node.AddChild(childDirNode);
                                }
                            }
                            else
                            {
                                totalSize += childDirSize;
                                node.AddChild(childDirNode);
                            }
                        }
                        else
                        {
                            if (syncObj != null)
                            {
                                lock (syncObj)
                                {
                                    totalSize += childDirSize;
                                }
                            }
                            else
                            {
                                totalSize += childDirSize;
                            }
                        }
                    }
                    else if (verbose)
                    {
                        Console.WriteLine($"Skipping directory due to max depth: {dirInfo.FullName}");
                    }
                }
            }
            catch (UnauthorizedAccessException uaEx)
            {
                if (verbose)
                    Console.Error.WriteLine($"Access denied for {entry.FullName}: {uaEx.Message}");
            }
            catch (Exception ex)
            {
                if (verbose)
                    Console.Error.WriteLine($"Error processing {entry.FullName}: {ex.Message}");
            }
        }
    }
}