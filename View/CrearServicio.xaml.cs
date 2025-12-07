using appointex.ViewModels;

namespace appointex;

public partial class CrearServicio : ContentPage
{
    public CrearServicio(CreateServiceViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }   
}
