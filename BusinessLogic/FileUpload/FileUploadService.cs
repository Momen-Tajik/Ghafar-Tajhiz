using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic.FileUpload
{
    public class FileUploadService : IFileUploadService
    {
        private readonly string _storagePath;

        private const long MaxFileSize = 5 * 1024 * 1024;

        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

        private static readonly HashSet<string> AllowedContentTypes =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

        public FileUploadService(IConfiguration configuration)
        {
            _storagePath =
                configuration["FileUpload:StoragePath"]
                ?? throw new InvalidOperationException(
                    "FileUpload:StoragePath is not configured.");
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("فایلی ارسال نشده است.");

            if (file.Length > MaxFileSize)
                throw new InvalidOperationException(
                    "حجم فایل نمی‌تواند بیشتر از 5 مگابایت باشد.");

            var extension =
                Path.GetExtension(file.FileName);

            if (!AllowedExtensions.Contains(extension))
                throw new InvalidOperationException(
                    "فرمت فایل مجاز نیست.");

            if (!AllowedContentTypes.Contains(file.ContentType))
                throw new InvalidOperationException(
                    "نوع فایل مجاز نیست.");

            Directory.CreateDirectory(_storagePath);

            var fileName =
                $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";

            var fullPath =
                Path.Combine(_storagePath, fileName);

            await using var stream = new FileStream(
                fullPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                64 * 1024,
                useAsync: true);

            await file.CopyToAsync(stream);

            return fileName;
        }

        public bool DeleteFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var safeFileName =
                Path.GetFileName(fileName);

            if (!string.Equals(
                    safeFileName,
                    fileName,
                    StringComparison.Ordinal))
            {
                return false;
            }

            var fullPath =
                Path.Combine(_storagePath, safeFileName);

            if (!File.Exists(fullPath))
                return false;

            File.Delete(fullPath);

            return true;
        }
    }
}