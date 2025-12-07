using appointex.ViewModels;

namespace appointex
{
    public partial class Solicitar : ContentPage
    {
        public Solicitar(RequestServiceViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}