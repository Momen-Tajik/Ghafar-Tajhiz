using Microsoft.AspNetCore.Http;

namespace BusinessLogic.FileUpload
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file);

        bool DeleteFile(string fileName);

        Task<string> UploadReceiptAsync(IFormFile file);

        bool DeleteReceipt(string fileName);
    }
}