using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using appointex.Models;
using appointex.Services;
using System.Collections.ObjectModel;

namespace appointex.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly SupabaseService _supabaseService;

        public DashboardViewModel(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
            Services = new ObservableCollection<AppService>();
        }
        [ObservableProperty]
        private bool _isRefreshing;
        // --- PROPIEDADES DE PERFIL ---
        [ObservableProperty] private string _actionButtonText;
        private int _currentUserRole;
        [ObservableProperty] private string _userName;
        [ObservableProperty] private string _userProfilePhoto;

        // --- PROPIEDADES DE ESTADO ---
        [ObservableProperty] private bool _isAddButtonVisible;
        [ObservableProperty] private string _emptyStateMessage;
        [ObservableProperty] private bool _isBusy;
        

        // --- COLECCIÓN DE SERVICIOS ---
        public ObservableCollection<AppService> Services { get; }


        // Método para iniciar (llamar desde OnAppearing en la Page)
        public async Task InitializeAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                // 1. OBTENER USUARIO ACTUAL
                var currentUserSession = _supabaseService.Client.Auth.CurrentUser;
                if (currentUserSession == null) return;

                // Traemos los datos extendidos del usuario (nombre, foto, rol)
                var userResponse = await _supabaseService.Client
                    .From<AppUser>()
                    .Where(x => x.UserId == currentUserSession.Id)
                    .Single();

                if (userResponse != null)
                {
                    UserName = userResponse.Username;
                    // Si no tiene foto, ponemos una por defecto o dejamos vacío
                    UserProfilePhoto = string.IsNullOrEmpty(userResponse.ProfilePhotoUrl) ? "user_placeholder.png" : userResponse.ProfilePhotoUrl;
                    _currentUserRole = userResponse.Role;
                    // 2. CONFIGURAR SEGÚN ROL
                    // Rol 1 = Cliente, Rol 2 = Socio
                    if (userResponse.Role == 2)
                    {
                        // ES SOCIO
                        IsAddButtonVisible = true;
                        ActionButtonText = "Editar";
                        EmptyStateMessage = "Agrega un servicio para comenzar a recibir clientes.";
                        await LoadServicesForPartner(userResponse.UserId);
                    }
                    else
                    {
                        // ES CLIENTE
                        IsAddButtonVisible = false;
                        ActionButtonText = "Solicitar";
                        EmptyStateMessage = "Parece que no hay ningún servicio disponible por ahora.";
                        await LoadServicesForClient();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error dashboard: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadServicesForPartner(string userId)
        {
            try
            {
                Services.Clear();
                // Traer solo los servicios creados por ESTE usuario
                var result = await _supabaseService.Client
                    .From<AppService>()
                    .Where(x => x.UserId == userId)
                    .Get();

                foreach (var item in result.Models) Services.Add(item);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Detalle: {ex.Message} Id: {userId}", "OK");
            }
        }

        private async Task LoadServicesForClient()
        {
            try
            {
                Services.Clear();
                // Traer TODOS los servicios disponibles + Datos del Creador (User)
                // Supabase en C# trae las relaciones automáticamente si usas [Reference]
                var result = await _supabaseService.Client
                    .From<AppService>()
                    .Select("*") // Esto asegura que traiga el objeto User anidado
                    .Get();

                foreach (var item in result.Models) Services.Add(item);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Detalle: {ex.Message}", "OK");
            }
        }

        // --- COMANDOS ---

        [RelayCommand]
        private async Task AddService()
        {
            // TODO: Agregar navegación a la página de crear servicio
            await Shell.Current.GoToAsync(nameof(CrearServicio));
        }


        [RelayCommand]
        private async Task RequestService(AppService service)
        {
            if (service == null) return;

            // CREAMOS EL DICCIONARIO DE PARÁMETROS
            var navigationParameter = new Dictionary<string, object>
            {
                { "ServiceObj", service } // "ServiceObj" debe coincidir con el QueryProperty del otro ViewModel
            };
            if (_currentUserRole == 2) // ES SOCIO -> VA A EDITAR
            {
                // Asegúrate de registrar esta ruta en AppShell
                await Shell.Current.GoToAsync(nameof(EditarServicio), navigationParameter);
            }
            else // ES CLIENTE -> VA A SOLICITAR
            {
                await Shell.Current.GoToAsync(nameof(Solicitar), navigationParameter);
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            // Activamos el estado de refresh visual
            IsRefreshing = true;

            try
            {
                // Reutilizamos tu lógica de carga existente
                await InitializeAsync();
            }
            finally
            {
                // Apagamos el spinner del refresh
                IsRefreshing = false;
            }
        }
    }
}