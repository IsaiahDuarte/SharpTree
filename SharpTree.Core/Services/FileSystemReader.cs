using SharpTree.Core.Models;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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

            var enumerationOptions = new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = false,
                AttributesToSkip = FileAttributes.ReparsePoint,
                ReturnSpecialDirectories = false
            };

            IEnumerable<FileSystemInfo> entries;
            try
            {
                entries = directoryInfo.EnumerateFileSystemInfos("*", enumerationOptions);
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
                try
                {
                    if (entry is FileInfo fileInfo)
                    {
                        if (verbose)
                            Console.WriteLine($"Processing file: {fileInfo.FullName}");
                        if (fileInfo.Length >= minSize)
                        {
                            lock (syncObj)
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
                        if (maxDepth == -1 || 0 < maxDepth)
                        {
                            var childDir = ReadRecursive(
                                dirInfo.FullName,
                                minSize,
                                maxDepth,
                                isRoot: false,
                                currentDepth: 1,
                                verbose);

                            if (childDir.Size > 0)
                            {
                                lock (syncObj)
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

            var enumerationOptions = new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = false,
                AttributesToSkip = FileAttributes.ReparsePoint,
                ReturnSpecialDirectories = false
            };

            IEnumerable<FileSystemInfo> entries;
            try
            {
                entries = directoryInfo.EnumerateFileSystemInfos("*", enumerationOptions);
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
                try
                {
                    if (entry is FileInfo fileInfo)
                    {
                        if (verbose)
                            Console.WriteLine($"Processing file: {fileInfo.FullName}");
                        if (fileInfo.Length >= minSize)
                        {
                            totalSize += fileInfo.Length;
                            node.AddChild(new FileNode(fileInfo.Name, fileInfo.Length));
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
                            totalSize += childDir.Size;
                            if (childDir.Size > 0)
                                node.AddChild(childDir);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (verbose)
                        Console.Error.WriteLine($"Error processing {entry.FullName}: {ex.Message}");
                }
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
    }
}