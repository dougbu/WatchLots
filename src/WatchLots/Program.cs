using System;
using System.Collections.Generic;
using System.IO;

namespace WatchLots
{
    public class Program
    {
        public int Main(string[] args)
        {
            var watchers = new List<FileSystemWatcher>();
            var current = Directory.GetCurrentDirectory();
            for (var i = 0; i < 25; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    foreach (var file in Directory.EnumerateFiles(current, "project.json", SearchOption.AllDirectories))
                    {
                        var directory = Path.GetDirectoryName(file);
                        var watcher = new FileSystemWatcher(directory)
                        {
                            IncludeSubdirectories = true,
                        };
                        watcher.Changed += OnChanged;
                        watcher.Created += OnChanged;
                        watcher.Deleted += OnChanged;
                        watcher.Error += OnError;
                        watcher.Renamed += OnRenamed;

                        try
                        {
                            watcher.EnableRaisingEvents = true;
                        }
                        catch (IOException exception)
                        {
                            Console.Error.WriteLine(
                                $"Caught Exception with HResult '{ exception.HResult }' after creating " +
                                $"{ watchers.Count } instances:" +
                                Environment.NewLine +
                                $"{ exception }");
                            return 1;
                        }

                        watchers.Add(watcher);
                    }
                }

                Console.WriteLine($"Completed { 10 * (i + 1) } iterations.");
            }

            return 0;
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File '{ e.FullPath }' had a '{ e.ChangeType }' change.");
        }

        private static void OnError(object sender, ErrorEventArgs e)
        {
            var exception = e.GetException();
            Console.Error.WriteLine($"Hit an Exception with HResult '{ exception.HResult }'" +
                Environment.NewLine +
                $"{ exception }");

            Environment.FailFast(exception.Message);
        }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"File '{ e.OldFullPath }' renamed ('{ e.ChangeType }') to '{ e.FullPath }'.");
        }
    }
}
