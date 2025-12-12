using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ghafar_Tajhiz_Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly string _storagePath;

        public FileUploadController(IConfiguration configuration)
        {
            _storagePath = configuration["FileUpload:StoragePath"];
        }

        [HttpGet("GetFile")]
        public IActionResult DownloadFile(string fileName)
        {
            var fullPath=Path.Combine(_storagePath, fileName);

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound("File Not Found");
            }
            var fileBytes=System.IO.File.ReadAllBytes(fullPath);
            var contentType = "application/octet-stream";
            return PhysicalFile(fullPath, contentType, fileName);
        }

        [HttpDelete("Delete")]
        public IActionResult DeleteFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("نام فایل ارسال نشده است.");

            var fullPath = Path.Combine(_storagePath, fileName);

            if (!System.IO.File.Exists(fullPath))
                return NotFound("File Not Found");

            System.IO.File.Delete(fullPath);

            return Ok("File Deleted Successfully");
        }
    }
}
