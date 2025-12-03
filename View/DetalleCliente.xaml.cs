using appointex.ViewModel;
using Microsoft.Maui.Controls;

namespace appointex.View
{
    public partial class DetalleCliente : ContentPage
    {
        public DetalleCliente()
        {
            InitializeComponent();
            this.BindingContext = new DetalleServicioViewModel();
        }
    }
}