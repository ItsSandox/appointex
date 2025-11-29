namespace appointex;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		//Forzar modo claro
		UserAppTheme = AppTheme.Light;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}