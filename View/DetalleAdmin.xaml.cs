using appointex.ViewModel;
using Microsoft.Maui.Controls;

namespace appointex.View
{
    public partial class DetalleAdmin : ContentPage
    {
        public DetalleAdmin()
        {
            InitializeComponent();
            this.BindingContext = new DetalleServicioViewModel();
        }
    }
}