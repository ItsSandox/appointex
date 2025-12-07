using appointex.ViewModels;

namespace appointex;
public partial class EditarServicio : ContentPage
{
    public EditarServicio(EditServiceViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}