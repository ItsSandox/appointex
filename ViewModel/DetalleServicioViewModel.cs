using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace appointex.ViewModel
{
    public partial class DetalleServicioViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task CompletarServicio()
        {
            await Application.Current.MainPage.DisplayAlert("Servicio Completado", "El estado del servicio ha sido marcado como Completado.", "OK");
            // Lógica para actualizar el estado del servicio
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}