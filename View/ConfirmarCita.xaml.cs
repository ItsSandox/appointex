using appointex.ViewModel;
using Microsoft.Maui.Controls;

namespace appointex.View
{
    public partial class ConfirmarCita : ContentPage
    {
        public ConfirmarCita()
        {
            InitializeComponent();
            // Asignamos el ViewModel de Confirmación (asumiendo que lo crearemos a continuación)
            this.BindingContext = new ConfirmarCitaViewModel();
        }
    }
}