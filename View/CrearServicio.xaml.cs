using appointex.ViewModel;
using Microsoft.Maui.Controls;

namespace appointex.View
{
    public partial class CrearServicio : ContentPage
    {
        public CrearServicio()
        {
            InitializeComponent();
            this.BindingContext = new CrearServicioViewModel();
        }
    }
}