using appointex.ViewModels;

namespace appointex;

public partial class PlansPage : ContentPage
{
	public PlansPage(PlansViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}