﻿using System;
using System.IO;

namespace SharpTree.Core.Behaviors
{
    public class SingleVolumeBehavior : IFilesystemBehavior
    {
        public string RootVolume { get; private set; }

        public SingleVolumeBehavior(string rootVolume)
        {
            if (string.IsNullOrWhiteSpace(rootVolume))
                throw new ArgumentException("Root volume must be a valid non-empty path.", nameof(rootVolume));

            string pathRoot = Path.GetPathRoot(rootVolume);
            if (string.IsNullOrEmpty(pathRoot))
                throw new ArgumentException("Invalid root volume path.", nameof(rootVolume));

            RootVolume = pathRoot;
        }

        public IFilesystemBehavior GetNextLevel(DirectoryInfo directory)
        {
            string currentVolume = Path.GetPathRoot(directory.FullName);
            return string.Equals(RootVolume, currentVolume, StringComparison.OrdinalIgnoreCase) ? this : null;
        }
    }
}