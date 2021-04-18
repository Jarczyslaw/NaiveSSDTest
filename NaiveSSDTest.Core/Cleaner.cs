using System.Collections.Generic;
using System.IO;

namespace NaiveSSDTest.Core
{
    public class Cleaner
    {
        private readonly List<string> toCleanup = new List<string>();

        public void AddToCleanup(string path)
        {
            if (!toCleanup.Contains(path))
            {
                toCleanup.Add(path);
            }
        }

        public void Cleanup()
        {
            foreach (var path in toCleanup)
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            toCleanup.Clear();
        }
    }
}