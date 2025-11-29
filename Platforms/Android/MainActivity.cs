using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View; // Necesario para WindowCompat

namespace appointex // He actualizado el namespace para coincidir con tu XAML
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Lógica para hacer la barra de navegación y estado transparentes y extender el contenido
            MakeStatusBarAndNavigationBarTransparent();
        }

        private void MakeStatusBarAndNavigationBarTransparent()
        {
            // Verificamos que la ventana no sea nula
            if (Window != null)
            {
                // 1. IMPORTANTE: Eliminar límites predeterminados para que el contenido fluya
                Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);

                // 2. Decirle a la ventana que el contenido debe extenderse detrás de las barras del sistema (Edge-to-Edge)
                WindowCompat.SetDecorFitsSystemWindows(Window, false);

                // 3. Limpiar banderas translúcidas antiguas que pueden bloquear la transparencia total
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Window.ClearFlags(WindowManagerFlags.TranslucentNavigation);

                // 4. Permitir dibujar los fondos de las barras del sistema
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

                // 5. Hacer las barras transparentes
                Window.SetNavigationBarColor(Android.Graphics.Color.Transparent);
                Window.SetStatusBarColor(Android.Graphics.Color.Transparent);

                // 6. Configurar el color de los iconos (oscuros o claros)
                var windowInsetsController = WindowCompat.GetInsetsController(Window, Window.DecorView);
                if (windowInsetsController != null)
                {
                    // true = Iconos oscuros (para fondos claros como tu Login)
                    // false = Iconos claros (para fondos oscuros)
                    windowInsetsController.AppearanceLightNavigationBars = true; 
                    windowInsetsController.AppearanceLightStatusBars = true;
                }
            }
        }
    }
}