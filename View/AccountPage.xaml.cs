using appointex.ViewModels;

namespace appointex;

public partial class AccountPage : ContentPage
{
	private readonly AccountViewModel _viewModel;
	public AccountPage(AccountViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
	protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
