using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using appointex.Services;
using appointex.Models;

namespace appointex.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        //Servicios
        private readonly SupabaseService _supabaseService;
        private readonly IFcmService _fcmService;
        public LoginViewModel(SupabaseService supabaseService, IFcmService fcmService)
        {
            _supabaseService = supabaseService;
            _fcmService = fcmService;
        }

        // Aquí puedes agregar propiedades para el Email y Password si necesitas validarlos
        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        // Propiedad para el ActivityIndicator
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private bool _isBusy;

        // --- COMANDO DE INICIO DE SESIÓN ---
        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Por favor ingresa correo y contraseña", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                var response = await _supabaseService.Client.Auth.SignIn(Email, Password);

                if (response?.User != null)
                {
                    // =======================================================
                    // 3. NUEVA LÓGICA: Obtener y Guardar Token FCM
                    // =======================================================
                    try
                    {
                        // A. Obtenemos el token del dispositivo
                        var token = await _fcmService.GetTokenAsync();

                        if (!string.IsNullOrEmpty(token))
                        {
                            // FORMA CORRECTA: Usamos .Set() para actualizar SOLO el token
                            await _supabaseService.Client
                                .From<AppUser>()
                                .Where(x => x.UserId == response.User.Id) // Importante: Filtramos por ID
                                .Set(x => x.FcmToken, token)              // Definimos qué campo cambia
                                .Update();                                // Ejecutamos la actualización

                            Console.WriteLine($"Token FCM actualizado para: {response.User.Id}");
                        }
                    }
                    catch (Exception exToken)
                    {
                        // No detenemos el login si falla el token, solo lo logueamos
                        Console.WriteLine($"Error guardando token FCM: {exToken.Message}");
                    }
                    // =======================================================

                    Password = string.Empty;
                    await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error de acceso", $"Credenciales incorrectas o error de conexión. {ex.Message}", "OK");
                Console.WriteLine($"Error Login: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Comando para volver atrás con la flecha
        [RelayCommand]
        private async Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}