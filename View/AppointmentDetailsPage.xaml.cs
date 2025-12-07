using appointex.ViewModels;
namespace appointex;

public partial class AppointmentDetailsPage : ContentPage
{
    public AppointmentDetailsPage(AppointmentDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}