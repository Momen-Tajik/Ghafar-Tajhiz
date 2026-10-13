namespace BusinessLogic.FileUpload
{
    public sealed class FileStorageSettings
    {
        public FileStorageSettings(string storagePath)
        {
            if (string.IsNullOrWhiteSpace(storagePath))
            {
                throw new ArgumentException(
                    "Storage path is required.",
                    nameof(storagePath));
            }

            StoragePath = Path.GetFullPath(storagePath);
        }

        public string StoragePath { get; }
    }
}
