﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace SocialApp.Api.Services
{
    public class PhotoUploadService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public PhotoUploadService(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }
        public  async Task <(string PhotoPath, string PhotoUrl)>  SavePhotoAsync(IFormFile photo , params string[] folderPaths)
        {
            var targetFolderPath = Path.Combine(new[] { _webHostEnvironment.WebRootPath }.Concat(folderPaths).ToArray());

            if (!Directory.Exists(targetFolderPath))
            {
                Directory.CreateDirectory(targetFolderPath);
            }

            var extension = Path.GetExtension(photo.FileName);
            var newPhotoName = $"{Guid.NewGuid()}_{DateTime.UtcNow.Ticks}{extension}";
            var fullPhotoPath = Path.Combine(targetFolderPath, newPhotoName);

            using (FileStream fs = File.Create(fullPhotoPath))
            {
                await photo.CopyToAsync(fs);
            }

            var domainUrl = _configuration.GetValue<string>("Domain")?.TrimEnd('/');
            var photoUrl = $"{domainUrl}/{string.Join('/',folderPaths)}/{newPhotoName}";

            return (fullPhotoPath, photoUrl);
        }
    }
}
