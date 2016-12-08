﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NullVoidCreations.Janitor.Shell.Engine
{
    class DirectoryWalker: IEnumerable<string>
    {
        string _rootDirectory;
        Func<string, bool> _directoryFilter, _fileFilter;

        public DirectoryWalker(string rootDirectory): this(rootDirectory, null, null)
        {

        }

        public DirectoryWalker(string rootDirectory, Func<string, bool> fileFilter): this(rootDirectory, null, fileFilter)
        {

        }

        public DirectoryWalker(string rootDirectory, Func<string, bool> directoryFilter, Func<string, bool> fileFilter)
        {
            _rootDirectory = rootDirectory;
            _directoryFilter = directoryFilter;
            _fileFilter = fileFilter;
        }

        public IEnumerator<string> GetEnumerator()
        {
            var directories = new Queue<string>();
            var files = new Queue<string>();

            directories.Enqueue(_rootDirectory);
            while (files.Count > 0 || directories.Count > 0)
            {
                if (files.Count > 0)
                    yield return files.Dequeue();

                if (directories.Count > 0)
                {
                    var directory = directories.Dequeue();

                    try
                    {
                        var subDirectories = Directory.GetDirectories(directory);
                        foreach (var path in subDirectories)
                        {
                            if (_directoryFilter == null || _directoryFilter(path))
                                directories.Enqueue(path);
                        }
                    }
                    catch
                    {

                    }

                    try
                    {
                        var subFiles = Directory.GetFiles(directory);
                        foreach (var path in subFiles)
                        {
                            if (_fileFilter == null || _fileFilter(path))
                                files.Enqueue(path);
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
