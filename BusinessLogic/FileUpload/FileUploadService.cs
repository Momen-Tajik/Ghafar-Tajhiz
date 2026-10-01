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


        // =========================================================
        // Product / General File Upload
        // =========================================================

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            return await UploadInternalAsync(
                file,
                _storagePath);
        }


        // =========================================================
        // Receipt Upload
        // =========================================================

        public async Task<string> UploadReceiptAsync(IFormFile file)
        {
            var receiptPath =
                Path.Combine(
                    _storagePath,
                    "receipts");

            return await UploadInternalAsync(
                file,
                receiptPath);
        }


        // =========================================================
        // Common Upload Logic
        // =========================================================

        private async Task<string> UploadInternalAsync(
            IFormFile file,
            string targetDirectory)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException(
                    "فایلی ارسال نشده است.");
            }


            if (file.Length > MaxFileSize)
            {
                throw new InvalidOperationException(
                    "حجم فایل نمی‌تواند بیشتر از 5 مگابایت باشد.");
            }


            var extension =
                Path.GetExtension(file.FileName);

            if (string.IsNullOrWhiteSpace(extension) ||
                !AllowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException(
                    "فرمت فایل مجاز نیست.");
            }


            if (string.IsNullOrWhiteSpace(file.ContentType) ||
                !AllowedContentTypes.Contains(file.ContentType))
            {
                throw new InvalidOperationException(
                    "نوع فایل مجاز نیست.");
            }


            Directory.CreateDirectory(
                targetDirectory);


            // نام تصادفی برای جلوگیری از تداخل
            var fileName =
                $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";


            var fullPath =
                Path.Combine(
                    targetDirectory,
                    fileName);


            await using var stream =
                new FileStream(
                    fullPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    64 * 1024,
                    useAsync: true);


            await file.CopyToAsync(stream);


            return fileName;
        }


        // =========================================================
        // Delete Product / General File
        // =========================================================

        public bool DeleteFile(string fileName)
        {
            return DeleteInternal(
                fileName,
                _storagePath);
        }


        // =========================================================
        // Delete Receipt
        // =========================================================

        public bool DeleteReceipt(string fileName)
        {
            var receiptPath =
                Path.Combine(
                    _storagePath,
                    "receipts");

            return DeleteInternal(
                fileName,
                receiptPath);
        }


        // =========================================================
        // Common Delete Logic
        // =========================================================

        private bool DeleteInternal(
            string fileName,
            string directory)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;


            // فقط نام فایل؛ بدون مسیر
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
                Path.Combine(
                    directory,
                    safeFileName);


            if (!File.Exists(fullPath))
                return false;


            File.Delete(fullPath);

            return true;
        }
    }
}