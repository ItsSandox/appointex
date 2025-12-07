using Microsoft.Extensions.Logging;
using appointex.ViewModels;
using appointex.Services;
#if ANDROID
using appointex.Platforms.Android;
#endif

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
        // Servicios
        builder.Services.AddSingleton<SupabaseService>();

        // REGISTRO DEL SERVICIO DE NOTIFICACIONES (NATIVO)
#if ANDROID
        builder.Services.AddSingleton<IFcmService, FcmService>();
#endif

        // Vistas y ViewModels
        builder.Services.AddSingleton<InicioPage>();
        builder.Services.AddSingleton<InicioViewModel>();
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<HistoryPage>();
        builder.Services.AddSingleton<HistoryViewModel>();
        builder.Services.AddSingleton<DashboardPage>();
        builder.Services.AddSingleton<DashboardViewModel>();
        builder.Services.AddSingleton<AccountPage>();
        builder.Services.AddSingleton<AccountViewModel>();

        builder.Services.AddTransient<AppointmentDetailsPage>();
        builder.Services.AddTransient<AppointmentDetailsViewModel>();
        builder.Services.AddTransient<RequestServiceViewModel>();
        builder.Services.AddTransient<Solicitar>();
        builder.Services.AddTransient<CreateAccountViewModel>();
        builder.Services.AddTransient<CreateAccountClientPage>();
        builder.Services.AddTransient<CreateAccountPartnerPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<CrearServicio>();
        builder.Services.AddTransient<CreateServiceViewModel>();

        return builder.Build();
    }
}