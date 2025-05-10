using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SocialApp.Api.Data.Entities
{
    public class Likes
    {
       
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
