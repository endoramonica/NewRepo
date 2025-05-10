using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
namespace SocialApp.Api.Data.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(300)]
        public string PasswordHash { get; set; }
        [Comment("physical path of the image")]

        public string? PhotoPath { get; set; } // so that we may move or delete 
        public string? PhotoUrl { get; set; } // so that it can be consumed by other apps 



    }
}
