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
			Routing.RegisterRoute(nameof(CrearServicio), typeof(CrearServicio));
			Routing.RegisterRoute(nameof(Solicitar), typeof(Solicitar));
			Routing.RegisterRoute(nameof(AppointmentDetailsPage), typeof(AppointmentDetailsPage));
			Routing.RegisterRoute(nameof(PlansPage),typeof(PlansPage));
			Routing.RegisterRoute(nameof(EditarServicio), typeof(EditarServicio));
	}
}
