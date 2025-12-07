using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace appointex.Models
{
    [Table("services")]
    public class AppService : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("service_photo")]
        public string ServicePhoto { get; set; }

        [Column("duration")]
        public TimeSpan Duration { get; set; }

        [Column("price")]
        public float Price { get; set; }

        // Relación con la tabla users (El dueño del servicio)
        [Reference(typeof(AppUser))]
        public AppUser User { get; set; }
    }
}