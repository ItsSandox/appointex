using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace appointex.Models
{
    [Table("appointments")]
    public class AppAppointment : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }

        [Column("service_id")]
        public int ServiceId { get; set; }

        [Column("client_id")] 
        public string ClientId { get; set; }

        [Column("date_time")]
        public DateTime DateTime { get; set; }

        [Column("note")]
        public string Note { get; set; }

        [Column("state")]
        public int State { get; set; } 
        // 1 = Pendiente, 2 = Aprobado, 3 = Rechazado, 4 = Cancelado, 5 = Completado

        // --- RELACIONES ---

        [Reference(typeof(AppService))]
        public AppService Service { get; set; }

        [Reference(typeof(AppUser))]
        public AppUser Client { get; set; }


        // --- PROPIEDADES VISUALES (Ignoradas en BD) ---

        [JsonIgnore] 
        public string DisplayName { get; set; } 

        [JsonIgnore]
        public string DisplayRoleLabel { get; set; }

        [JsonIgnore]
        public Color StatusColor => State switch
        {
            1 => Color.FromArgb("#fec84b"), // Pendiente (Amarillo)
            2 => Color.FromArgb("#32d583"), // Aprobado/Completado (Verde)
            3 => Color.FromArgb("#f97066"), // Rechazado (Rojo)
            4 => Color.FromArgb("#f97066"), // Cancelado (Rojo)
            5 => Color.FromArgb("#32d583"), // Completado (Verde)
            _ => Colors.Gray
        };

        [JsonIgnore]
        public string StatusText => State switch
        {
            1 => "Pendiente",
            2 => "Aprobado",
            3 => "Rechazado",
            4 => "Cancelado",
            5 => "Completado",
            _ => "Desconocido"
        };
    }
}