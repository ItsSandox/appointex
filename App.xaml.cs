namespace appointex;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        UserAppTheme = AppTheme.Light;

        // Mostrar SOLO Solicitar como pantalla inicial
        MainPage = new View.DetalleCliente();
    }



//	public App()
//	{
//		InitializeComponent();
//		//Forzar modo claro
//		UserAppTheme = AppTheme.Light;
//	}

//	protected override Window CreateWindow(IActivationState? activationState)
//	{
//		return new Window(new AppShell());
//	}
}

