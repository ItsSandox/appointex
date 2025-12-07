using appointex.ViewModels;

namespace appointex;

public partial class DashboardPage : ContentPage
{
	private readonly DashboardViewModel _viewModel;
	public DashboardPage(DashboardViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		// Carga los datos cada vez que entras a la pantalla
		await _viewModel.InitializeAsync();
	}
}
