namespace BusinessLogic.FileUpload
{
    public static class FileStoragePath
    {
        public static string Resolve(
            string contentRootPath,
            string? configured)
        {
            if (string.IsNullOrWhiteSpace(configured))
            {
                throw new InvalidOperationException(
                    "FileUpload:StoragePath is not configured.");
            }

            var path = Path.IsPathFullyQualified(configured)
                ? configured
                : Path.Combine(contentRootPath, configured);

            return Path.GetFullPath(path);
        }

        public static bool IsInsideStorage(
            string storageRoot,
            string candidatePath)
        {
            var root = Path.GetFullPath(storageRoot);

            if (!root.EndsWith(Path.DirectorySeparatorChar))
            {
                root += Path.DirectorySeparatorChar;
            }

            var fullPath = Path.GetFullPath(candidatePath);

            return fullPath.StartsWith(
                root,
                StringComparison.OrdinalIgnoreCase);
        }

        public static string GetContentType(string path)
        {
            var extension =
                Path.GetExtension(path)
                    .ToLowerInvariant();

            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}
