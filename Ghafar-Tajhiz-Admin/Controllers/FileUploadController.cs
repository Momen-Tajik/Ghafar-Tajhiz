using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ghafar_Tajhiz_Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly string _storagePath;

        public FileUploadController(IConfiguration configuration)
        {
            _storagePath =
                configuration["FileUpload:StoragePath"]
                ?? throw new InvalidOperationException(
                    "FileUpload:StoragePath is not configured.");
        }

        [AllowAnonymous]
        [HttpGet("GetFile")]
        public IActionResult GetFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("نام فایل ارسال نشده است.");

            var safeFileName =
                Path.GetFileName(fileName);

            if (!string.Equals(
                    safeFileName,
                    fileName,
                    StringComparison.Ordinal))
            {
                return BadRequest("نام فایل نامعتبر است.");
            }

            var fullPath =
                Path.Combine(_storagePath, safeFileName);

            if (!System.IO.File.Exists(fullPath))
                return NotFound("File Not Found");

            var extension =
                Path.GetExtension(safeFileName);

            var contentType =
                extension.ToLowerInvariant() switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };

            return PhysicalFile(
                fullPath,
                contentType);
        }

        [HttpDelete("Delete")]
        public IActionResult DeleteFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest(
                    "نام فایل ارسال نشده است.");

            var safeFileName =
                Path.GetFileName(fileName);

            if (!string.Equals(
                    safeFileName,
                    fileName,
                    StringComparison.Ordinal))
            {
                return BadRequest(
                    "نام فایل نامعتبر است.");
            }

            var fullPath =
                Path.Combine(_storagePath, safeFileName);

            if (!System.IO.File.Exists(fullPath))
                return NotFound("File Not Found");

            System.IO.File.Delete(fullPath);

            return Ok(new
            {
                res = true,
                msg = "File Deleted Successfully"
            });
        }
    }
}