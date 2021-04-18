using NaiveSSDTest.Core;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NaiveSSDTest.ConsoleTest
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            AsyncMain()
                .GetAwaiter()
                .GetResult();
        }

        private static async Task AsyncMain()
        {
            var dataManager = new DataManager()
            {
                TasksCount = 8,
                ForceFilesCreation = true
            };

            var copyManager = new CopyManager()
            {
                Random = true,
                TasksCount = 16
            };

            var handler = new FileHandler();
            try
            {
                var config = new Configuration("C://");

                var stopwatch = Stopwatch.StartNew();

                await dataManager.CreateData(config, handler);

                Console.WriteLine($"Files created in {stopwatch.Elapsed.TotalSeconds}s");
                Console.WriteLine("Press enter to start copy process");
                Console.ReadLine();

                stopwatch = Stopwatch.StartNew();

                await copyManager.Copy(config, handler);

                Console.WriteLine($"Files copied in {stopwatch.Elapsed.TotalSeconds}s");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            finally
            {
                Console.WriteLine("Cleaning up...");
                dataManager.Cleaner.Cleanup();
                copyManager.Cleaner.Cleanup();
                Console.WriteLine("Cleaned");
            }
            Console.ReadKey();
        }

        private class FileHandler : IFilesCopyHandler, IFilesCreationHandler
        {
            private int divider = 100;

            public void FileCopied(FileProgress progress)
            {
                if (progress.CheckUpdate(divider))
                {
                    Console.WriteLine($"Copied {progress.CurrentCount}/{progress.TotalCount} files, {progress.CurrentSize}/{progress.TotalSize} bytes");
                }
            }

            public void FileCreated(FileProgress progress)
            {
                if (progress.CheckUpdate(divider))
                {
                    Console.WriteLine($"Created {progress.CurrentCount}/{progress.TotalCount} files, {progress.CurrentSize}/{progress.TotalSize} bytes");
                }
            }

            public void FilesCopyFinished()
            {
                Console.WriteLine("Coping finished");
            }

            public void FilesCopyStarted(FileProgress progress)
            {
                Console.WriteLine("Coping started");
            }

            public void FilesCreationFinished()
            {
                Console.WriteLine("Creation finished");
            }

            public void FilesCreationStarted(FileProgress progress)
            {
                Console.WriteLine("Creation started");
            }
        }
    }
}