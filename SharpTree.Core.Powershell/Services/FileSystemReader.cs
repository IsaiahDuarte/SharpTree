using System;
using System.Collections.Generic;
using System.IO;
using SharpTree.Core.Behaviors;
using SharpTree.Core.Models;

namespace SharpTree.Core.Services
{
    public static class FileSystemReader
    {
        public static INode ReadRecursive(string path, bool followSymlinks, IFilesystemBehaviour fsBehaviour)
        {
            var directoryInfo = new DirectoryInfo(path);
            var node = new DirectoryNode(directoryInfo.Name);

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
                    if (entry is FileInfo fileInfo)
                    {
                        node.AddChild(new FileNode(fileInfo.Name, fileInfo.Length));
                    }
                    else if (entry is DirectoryInfo dirInfo)
                    {
                        var newFsBehaviour = fsBehaviour.GetNextLevel(dirInfo);
                        if (newFsBehaviour != null)
                        {
                            var childDir = ReadRecursive(dirInfo.FullName, followSymlinks, newFsBehaviour);
                            node.AddChild(childDir);
                        }
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