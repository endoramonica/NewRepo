using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialAppLibrary.Shared.Dtos.ChatDto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsOnline { get; set; }
        public bool IsAway { get; set; }
        public string? AwayDuration { get; set; }
        public DateTime? LastLogonTime { get; set; }
    }
}
