namespace NaiveSSDTest.Core
{
    public interface IFilesCreationHandler
    {
        void FilesCreationStarted(FileProgress progress);

        void FilesCreationFinished();

        void FileCreated(FileProgress progress);
    }
}