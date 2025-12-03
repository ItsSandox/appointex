using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace appointex.ViewModel
{
    public partial class ConfirmarCitaViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task ConfirmarCita()
        {
            // Lógica para confirmar la cita
            System.Diagnostics.Debug.WriteLine("=== Cita confirmada con fecha y notas! ===");
            await Application.Current.MainPage.DisplayAlert("Cita Confirmada", "Tu cita ha sido agendada con éxito.", "OK");

            // Después de confirmar, podríamos navegar a la página principal:
            // await Shell.Current.GoToAsync("//MainPage");
        }

        [RelayCommand]
        private async Task GoBack()
        {
            // Navegación hacia atrás, regresando a la pantalla de Solicitud de Servicio
            await Shell.Current.GoToAsync("..");
        }
    }
}