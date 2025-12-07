using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using appointex.Services;
using appointex.Models;

namespace appointex.ViewModels
{
    public partial class AccountViewModel : ObservableObject
    {
        private readonly SupabaseService _supabaseService;

        public AccountViewModel(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        // --- Propiedades del Perfil ---
        [ObservableProperty] private string _userName;
        [ObservableProperty] private string _userEmail;
        [ObservableProperty] private string _userPhoto;

        // --- Método de Inicialización (Llamar en OnAppearing) ---
        public async Task InitializeAsync()
        {
            try
            {
                var currentUser = _supabaseService.Client.Auth.CurrentUser;
                if (currentUser != null)
                {
                    UserEmail = currentUser.Email;

                    // Intentamos traer datos extra de la tabla 'users'
                    var userProfile = await _supabaseService.Client
                        .From<AppUser>()
                        .Where(u => u.UserId == currentUser.Id)
                        .Single();

                    if (userProfile != null)
                    {
                        UserName = userProfile.Username;
                        UserPhoto = !string.IsNullOrEmpty(userProfile.ProfilePhotoUrl) 
                                    ? userProfile.ProfilePhotoUrl 
                                    : "user_placeholder.png";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando perfil: {ex.Message}");
            }
        }

        // --- COMANDO: NOTIFICACIONES (Placeholder) ---
        [RelayCommand]
        private async Task GoToNotificationsOption()
        {
            // TODO: Aquí navegaremos a la página de configuración de notificaciones
            await Shell.Current.DisplayAlert("Notificaciones", "Próximamente podrás desactivarlas aquí.", "OK");
        }

        // --- COMANDO: CERRAR SESIÓN ---
        [RelayCommand]
        private async Task Logout()
        {
            bool confirm = await Shell.Current.DisplayAlert("Cerrar Sesión", "¿Estás seguro que deseas salir?", "Sí", "Cancelar");
            if (!confirm) return;

            try
            {
                await _supabaseService.Client.Auth.SignOut();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Reiniciar la navegación a la Login Page
                Application.Current.MainPage = new AppShell();
            }
        }
    }
}