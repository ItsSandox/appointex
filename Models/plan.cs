using System.Collections.ObjectModel;

namespace appointex.Models
{
    public class PlanModel
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string Period { get; set; } // Ej: "/mes"
        public string Description { get; set; }
        public bool IsPopular { get; set; } // Para resaltar el plan del medio
        public Color HeaderColor { get; set; }
        public Color ButtonColor { get; set; }
        public Color TextColor { get; set; }
        
        // Lista de ventajas
        public ObservableCollection<string> Features { get; set; }
    }
}