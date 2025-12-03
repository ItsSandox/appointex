using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace appointex.ViewModel
{
    public partial class CrearServicioViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task ElegirFoto()
        {
            await Application.Current.MainPage.DisplayAlert("Función Pendiente", "Se abriría el selector de fotos aquí.", "OK");
        }

        [RelayCommand]
        private async Task ConfirmarCreacion()
        {
            await Application.Current.MainPage.DisplayAlert("Creación Exitosa", "Nuevo servicio creado y publicado.", "OK");
            // Lógica para guardar el servicio y navegar a la lista
            // await Shell.Current.GoToAsync(".."); 
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}