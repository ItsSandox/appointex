using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace appointex.Models
{
    [Table("users")]
    public class AppUser : BaseModel
    {
        // El ID es un UUID que viene de Auth, por lo que lo insertamos manualmente (true)
        [PrimaryKey("id", true)] 
        public string UserId { get; set; } 

        [Column("username")]
        public string Username { get; set; }

        [Column("role")]
        public int Role { get; set; } // 1: Cliente, 2: Socio
        
        [Column("profile_photo")]
        public string? ProfilePhotoUrl { get; set; }

        [Column("profession")]
        public string? Profession { get; set; }

        [Column("fcm_token")]
        public string? FcmToken { get; set; }
    }
}