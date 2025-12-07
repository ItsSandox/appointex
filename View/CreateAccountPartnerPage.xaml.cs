using appointex.ViewModels;

namespace appointex;

public partial class CreateAccountPartnerPage : ContentPage
{
    public CreateAccountPartnerPage(CreateAccountViewModel viewModel)
    {
        InitializeComponent();
        
        viewModel.Role = 2; 
        
        BindingContext = viewModel;
    }
}