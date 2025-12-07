using appointex.ViewModels;

namespace appointex;

public partial class CreateAccountClientPage : ContentPage
{
    public CreateAccountClientPage(CreateAccountViewModel viewModel)
    {
        InitializeComponent();
        
        viewModel.Role = 1; 
        
        BindingContext = viewModel;
    }
}
