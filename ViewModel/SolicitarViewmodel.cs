using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace appointex.ViewModel
{
    // Asegúrate de que tu proyecto tenga instalado el paquete 'CommunityToolkit.Mvvm'
    public partial class SolicitarViewModel : ObservableObject
    {
        // 1. Comando para el botón "Solicitar"
        [RelayCommand]
        private async Task Solicitar()
        {
            // Lógica que se ejecuta cuando se presiona el botón "Solicitar"
            // Aquí iría:
            // 1. Validar datos (si los hubiera)
            // 2. Llamar a un servicio (ej. API)
            // 3. Navegar a la siguiente pantalla (ej. confirmación)

            // Ejemplo de mensaje de consola
            System.Diagnostics.Debug.WriteLine("=== Servicio solicitado exitosamente! ===");

            // Ejemplo: Navegar a una página de confirmación (asumiendo que existe)
            // await Shell.Current.GoToAsync("//ConfirmationPage"); 

            // Usamos un mensaje de alerta simple por ahora, ya que no tenemos otras páginas.
            await Application.Current.MainPage.DisplayAlert("Éxito", "Tu solicitud ha sido enviada.", "OK");
        }

        // 2. Comando para el botón de regreso (la flecha)
        [RelayCommand]
        private async Task GoBack()
        {
            // Lógica para navegar hacia atrás en la pila de navegación de Shell
            await Shell.Current.GoToAsync("..");
        }
    }
}