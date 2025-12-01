using Microsoft.Extensions.Logging;
using appointex.ViewModels;
using appointex;

namespace appointex;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("Montserrat-Regular.ttf", "Montserrat-Regular");
				fonts.AddFont("Montserrat-SemiBold.ttf", "Montserrat-SemiBold");
				fonts.AddFont("Montserrat-Bold.ttf", "Montserrat-Bold");
				fonts.AddFont("Montserrat-Medium.ttf", "Montserrat-Medium");
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
		builder.Services.AddSingleton<InicioPage>();
        builder.Services.AddSingleton<InicioViewModel>();
        builder.Services.AddSingleton<LoginViewModel>();
		builder.Services.AddSingleton<HistoryPage>();
		builder.Services.AddSingleton<DashboardPage>();
		builder.Services.AddSingleton<AccountPage>();

		builder.Services.AddTransient<CreateAccountClientPage>();
		builder.Services.AddTransient<CreateAccountPartnerPage>();
		builder.Services.AddTransient<LoginPage>();

		return builder.Build();

		
	}
}
