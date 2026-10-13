using BusinessLogic.FileUpload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ghafar_Tajhiz_Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;

        public FileUploadController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [HttpGet("GetFile")]
        public IActionResult DownloadFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("نام فایل ارسال نشده است.");

            var fullPath =
                _fileUploadService.GetAdminAccessibleFilePath(fileName);

            if (fullPath == null)
                return NotFound("File Not Found");

            return PhysicalFile(
                fullPath,
                FileStoragePath.GetContentType(fullPath));
        }

        [HttpDelete("Delete")]
        public IActionResult DeleteFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest(
                    "نام فایل ارسال نشده است.");
            }

            var deleted =
                _fileUploadService.DeleteFile(fileName);

            if (!deleted)
                return NotFound("File Not Found");

            return Ok(new
            {
                res = true,
                msg = "File Deleted Successfully"
            });
        }
    }
}
