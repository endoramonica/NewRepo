using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialAppLibrary.Shared.Dtos.ChatDto
{
    public abstract class BaseResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
    }
}
