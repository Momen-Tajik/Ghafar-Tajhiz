using BusinessLogic.FileUpload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ghafar_Tajhiz.Controllers
{
    [AllowAnonymous]
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
            var fullPath =
                _fileUploadService.GetPublicImagePath(fileName);

            if (fullPath == null)
                return NotFound("File Not Found");

            return PhysicalFile(
                fullPath,
                FileStoragePath.GetContentType(fullPath));
        }
    }
}
