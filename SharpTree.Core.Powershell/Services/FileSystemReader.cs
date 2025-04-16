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
        public static INode Read(string path, long minSize = 0, int maxDepth = -1, bool verbose = false, bool onlyDirectories = false)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException(string.Format("Path {0} does not exist", path), "path");
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
                    Console.Error.WriteLine(string.Format("Error reading root {0}: {1}", path, ex.Message));
                return new RootNode(directoryInfo.Name, 0, new List<INode>());
            }
            var syncObj = new object();
            Parallel.ForEach(entries, entry =>
            {
                ProcessEntry(entry, processingNode, minSize, maxDepth, verbose, ref totalSize, syncObj, 0, onlyDirectories);
            });
            processingNode.Size = totalSize;
            processingNode.SortChildren();
            if (verbose)
                Console.WriteLine(string.Format("Processed Root: {0} with Size: {1}", processingNode.Name, processingNode.Size));
            return new RootNode(processingNode.Name, processingNode.Size, processingNode.Children);
        }

        private static DirectoryNode ReadRecursive(string path, long minSize, int maxDepth, int currentDepth, bool verbose, bool onlyDirectories)
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
                    Console.Error.WriteLine(string.Format("Error reading {0}: {1}", path, ex.Message));
                node.Size = 0;
                return node;
            }
            foreach (var entry in entries)
            {
                ProcessEntry(entry, node, minSize, maxDepth, verbose, ref totalSize, null, currentDepth, onlyDirectories);
            }
            node.Size = totalSize;
            node.SortChildren();
            if (verbose)
                Console.WriteLine(string.Format("Returning DirectoryNode: {0} with Size: {1}", node.Name, node.Size));
            return node;
        }

        private static void ProcessEntry(FileSystemInfo entry,
                                         DirectoryNode node,
                                         long minSize,
                                         int maxDepth,
                                         bool verbose,
                                         ref long totalSize,
                                         object syncObj,
                                         int currentDepth,
                                         bool onlyDirectories)
        {
            try
            {
                if ((entry.Attributes & FileAttributes.ReparsePoint) != 0)
                {
                    if (verbose)
                        Console.WriteLine(string.Format("Skipping reparse point: {0}", entry.FullName));
                    return;
                }
                if (entry is FileInfo)
                {
                    FileInfo fileInfo = (FileInfo)entry;
                    if (verbose)
                        Console.WriteLine(string.Format("Processing file: {0}", fileInfo.FullName));
                    long currentFileSize = 0;
                    try
                    {
                        currentFileSize = fileInfo.Length;
                    }
                    catch (FileNotFoundException fnfEx)
                    {
                        if (verbose)
                            Console.Error.WriteLine(string.Format("Error accessing file {0}: {1}", fileInfo.FullName, fnfEx.Message));
                        return;
                    }
                    catch (IOException ioEx)
                    {
                        if (verbose)
                            Console.Error.WriteLine(string.Format("IO Error accessing file {0}: {1}", fileInfo.FullName, ioEx.Message));
                        return;
                    }
                    if (syncObj != null)
                    {
                        lock (syncObj)
                        {
                            totalSize += currentFileSize;
                            if (!onlyDirectories && currentFileSize >= minSize)
                                node.AddChild(new FileNode(fileInfo.Name, currentFileSize));
                        }
                    }
                    else
                    {
                        totalSize += currentFileSize;
                        if (!onlyDirectories && currentFileSize >= minSize)
                            node.AddChild(new FileNode(fileInfo.Name, currentFileSize));
                    }
                }
                else if (entry is DirectoryInfo)
                {
                    DirectoryInfo dirInfo = (DirectoryInfo)entry;
                    if (verbose)
                        Console.WriteLine(string.Format("Processing directory: {0}", dirInfo.FullName));
                    if (maxDepth == -1 || currentDepth < maxDepth)
                    {
                        var childDirNode = ReadRecursive(dirInfo.FullName, minSize, maxDepth, currentDepth + 1, verbose, onlyDirectories);
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
                        Console.WriteLine(string.Format("Skipping directory due to max depth: {0}", dirInfo.FullName));
                    }
                }
            }
            catch (UnauthorizedAccessException uaEx)
            {
                if (verbose)
                    Console.Error.WriteLine(string.Format("Access denied for {0}: {1}", entry.FullName, uaEx.Message));
            }
            catch (Exception ex)
            {
                if (verbose)
                    Console.Error.WriteLine(string.Format("Error processing {0}: {1}", entry.FullName, ex.Message));
            }
        }
    }
}