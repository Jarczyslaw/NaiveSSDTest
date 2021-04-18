namespace NaiveSSDTest.Core
{
    public interface IFilesCopyHandler
    {
        void FilesCopyFinished();

        void FileCopied(FileProgress progress);

        void FilesCopyStarted(FileProgress progress);
    }
}