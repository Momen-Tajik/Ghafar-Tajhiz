using Microsoft.AspNetCore.Http;

namespace BusinessLogic.FileUpload
{
    public class FileUploadService : IFileUploadService
    {
        private readonly string _storagePath;

        // ساختار پوشه‌ها داخل StoragePath (uploads):
        //   uploads/product-images  -> عکس محصولات (عمومی)
        //   uploads/receipts        -> رسیدهای پرداخت (خصوصی، فقط ادمین)
        private const string ProductImagesFolder = "product-images";
        private const string ReceiptsFolder = "receipts";

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


        public FileUploadService(FileStorageSettings settings)
        {
            _storagePath = settings.StoragePath;
        }


        private string ProductImagesPath =>
            Path.Combine(_storagePath, ProductImagesFolder);

        private string ReceiptsPath =>
            Path.Combine(_storagePath, ReceiptsFolder);


        // =====================================================
        // Upload
        // =====================================================

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            return await UploadInternalAsync(
                file,
                ProductImagesPath);
        }


        public async Task<string> UploadReceiptAsync(IFormFile file)
        {
            return await UploadInternalAsync(
                file,
                ReceiptsPath);
        }


        // =====================================================
        // Read
        // =====================================================

        /// <summary>
        /// فقط عکس محصول را برمی‌گرداند (عمومی).
        /// رسیدها از این متد هرگز در دسترس نیستند.
        /// </summary>
        public string? GetPublicImagePath(string? fileName)
        {
            var safeFileName = GetSafeFileName(fileName);

            if (safeFileName == null)
                return null;

            return GetExistingPathInsideStorage(
                Path.Combine(ProductImagesFolder, safeFileName));
        }


        /// <summary>
        /// برای پنل ادمین:
        /// "receipts/xxx.jpg" -> رسید
        /// "xxx.jpg"          -> عکس محصول
        /// </summary>
        public string? GetAdminAccessibleFilePath(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            var normalized =
                fileName
                    .Replace('\\', '/')
                    .Trim();

            if (normalized.Contains("..", StringComparison.Ordinal))
                return null;

            string relativePath;

            if (normalized.StartsWith(
                    ReceiptsFolder + "/",
                    StringComparison.OrdinalIgnoreCase))
            {
                var receiptName =
                    Path.GetFileName(normalized);

                if (string.IsNullOrWhiteSpace(receiptName))
                    return null;

                relativePath =
                    Path.Combine(ReceiptsFolder, receiptName);
            }
            else
            {
                var safeFileName = GetSafeFileName(normalized);

                if (safeFileName == null)
                    return null;

                relativePath =
                    Path.Combine(ProductImagesFolder, safeFileName);
            }

            return GetExistingPathInsideStorage(relativePath);
        }


        // =====================================================
        // Delete
        // =====================================================

        public bool DeleteFile(string fileName)
        {
            return DeleteInternal(
                fileName,
                ProductImagesPath);
        }


        public bool DeleteReceipt(string fileName)
        {
            return DeleteInternal(
                fileName,
                ReceiptsPath);
        }


        // =====================================================
        // Internals
        // =====================================================

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


        private bool DeleteInternal(
            string fileName,
            string directory)
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
                Path.Combine(
                    directory,
                    safeFileName);

            if (!FileStoragePath.IsInsideStorage(
                    directory,
                    fullPath))
            {
                return false;
            }


            if (!File.Exists(fullPath))
                return false;


            File.Delete(fullPath);

            return true;
        }


        private static string? GetSafeFileName(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            var safeFileName =
                Path.GetFileName(fileName.Trim());

            if (string.IsNullOrWhiteSpace(safeFileName) ||
                !string.Equals(
                    safeFileName,
                    fileName.Trim().Replace('\\', '/'),
                    StringComparison.Ordinal))
            {
                return null;
            }

            return safeFileName;
        }


        private string? GetExistingPathInsideStorage(string relativePath)
        {
            var fullPath =
                Path.GetFullPath(
                    Path.Combine(_storagePath, relativePath));

            if (!FileStoragePath.IsInsideStorage(
                    _storagePath,
                    fullPath))
            {
                return null;
            }

            if (!File.Exists(fullPath))
                return null;

            return fullPath;
        }
    }
}