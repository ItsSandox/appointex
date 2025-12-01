using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace appointex.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        public LoginViewModel()
        {
        }

        // Aquí puedes agregar propiedades para el Email y Password si necesitas validarlos
        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        // --- COMANDO DE INICIO DE SESIÓN ---
        [RelayCommand]
        private async Task Login()
        {
            // 1. Aquí iría tu lógica de validación (API, Firebase, etc.)
            // if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password)) ...

            // 2. Navegar al Dashboard
            // Nota: Usamos "///" si el Dashboard es la página principal para borrar el historial de navegación
            // O usamos nameof(DashboardPage) para navegación normal.
            
            // Opción A: Navegación simple (guarda historial, puedes volver atrás)
            await Shell.Current.GoToAsync(nameof(DashboardPage));

            // Opción B: Navegación absoluta (Borra el historial, no puedes volver al login con 'atrás')
            // await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
        }

        // Comando para volver atrás con la flecha
        [RelayCommand]
        private async Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}