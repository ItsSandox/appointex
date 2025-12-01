namespace appointex;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		    Routing.RegisterRoute(nameof(InicioPage), typeof(InicioPage));
		    Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(CreateAccountClientPage), typeof(CreateAccountClientPage));
            Routing.RegisterRoute(nameof(CreateAccountPartnerPage), typeof(CreateAccountPartnerPage));
	}
}
