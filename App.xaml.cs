using appointex.Services;

namespace appointex;

public partial class App : Application
{
	public App(SupabaseService supabaseService)
	{
		InitializeComponent();
		//Forzar modo claro
		UserAppTheme = AppTheme.Light;

		_ = InitializeSupabase(supabaseService);
	}
	private async Task InitializeSupabase(SupabaseService service)
    {
        try 
        {
            await service.InitializeAsync();
        }
        catch (Exception ex)
        {
           await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Error al inicializar Supabase: {ex.Message} ",
                    "Aceptar");
        }
    }
	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}