using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.IKEA.BLL.Common.Services.Attachments
{
    internal class AttachmentService : IAttachmentService
    {
        private readonly List<string> _allowedExtensions = new () { ".png", ".jpg", ".jpeg"};
        private const int _allowedMaxSize = 2_097_152;
        public string? Upload(IFormFile file, string folderName)
        {
            var extension = Path.GetExtension(file.FileName);     // "ahmed.png"

            if (!_allowedExtensions.Contains(extension))
                return null;

            if (file.Length == _allowedMaxSize)
                return null;

            //var folderPath = $"{Directory.GetCurrentDirectory}\\wwwroot\\files\\{folderName}";

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", folderName);

            if (Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{extension}";      // Must be unique

            var filePath = Path.Combine(folderPath, fileName);  // File Location Placed

            // Streaming : Data Per Time
            using var fileStream = new FileStream(filePath, FileMode.Create);
            //using var fileStream = File.Create(filePath);

            file.CopyTo(fileStream);

            return fileName;
        }

        public bool Delete(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
        
    }
}
