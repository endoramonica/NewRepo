using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialAppLibrary.Shared.Dtos
{
    public class SavePostRequest
    {
        public IFormFile? Photo { get; set; }
        public string SerializedSavePostDto { get; set; }
    }
}
