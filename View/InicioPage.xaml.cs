using appointex.ViewModels;

namespace appointex;

public partial class InicioPage : ContentPage
{
	public InicioPage()
	{
		InitializeComponent();
		 BindingContext = new InicioViewModel();
	}
}
