using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using appointex.Models;
using appointex.Services;
using System.Collections.ObjectModel;

namespace appointex.ViewModels
{
    public partial class HistoryViewModel : ObservableObject
    {
        private readonly SupabaseService _supabaseService;

        public HistoryViewModel(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
            Appointments = new ObservableCollection<AppAppointment>();
        }

        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private bool _isRefreshing;
        [ObservableProperty] private string _emptyMessage;

        public ObservableCollection<AppAppointment> Appointments { get; }

        public async Task InitializeAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await LoadData();
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            IsRefreshing = true;
            await LoadData();
            IsRefreshing = false;
        }

        private async Task LoadData()
        {
            var currentUser = _supabaseService.Client.Auth.CurrentUser;
            if (currentUser == null) return;

            Appointments.Clear();

            // 1. Averiguar mi ROL (Consultamos tabla users)
            var myProfile = await _supabaseService.Client
                .From<AppUser>()
                .Where(u => u.UserId == currentUser.Id)
                .Single();

            if (myProfile == null) return;

            // 2. Traer citas (La consulta es compleja porque trae datos anidados)
            // Traemos: Cita -> Servicio -> Socio (Dueño)
            // Traemos: Cita -> Cliente
            var result = await _supabaseService.Client
                .From<AppAppointment>()
                // Sintaxis avanzada: 
                // service:services(...) -> Trae el servicio y dentro el usuario dueño (users)
                // client:users!appointments_client_id_fkey(*) -> Trae al cliente usando la FK específica
                .Select("*, service:services(*, users(*)), client:users!appointments_client_id_fkey(*)")
                .Order("date_time", Supabase.Postgrest.Constants.Ordering.Descending) // Más recientes primero
                .Get();

            var allAppointments = result.Models;

            // 3. Filtrar y Procesar Nombres según Rol
            foreach (var appt in allAppointments)
            {
                // NOTA: Gracias a las políticas RLS que creamos antes, 
                // Supabase ya solo me devuelve las citas que tengo permiso de ver.
                // Aquí solo configuramos "qué texto mostrar".

                if (myProfile.Role == 1) // SOY CLIENTE
                {
                    // Quiero ver el nombre del SOCIO y el SERVICIO
                    appt.DisplayRoleLabel = "Socio";
                    appt.DisplayName = appt.Service?.User?.Username ?? "Desconocido";
                    Appointments.Add(appt);
                }
                else if (myProfile.Role == 2) // SOY SOCIO
                {
                    // Quiero ver el nombre del CLIENTE
                    appt.DisplayRoleLabel = "Cliente";
                    appt.DisplayName = appt.Client?.Username ?? "Desconocido";
                    Appointments.Add(appt);
                }
            }

            if (Appointments.Count == 0)
                EmptyMessage = "No tienes citas registradas aún.";
            else
                EmptyMessage = "";
        }

        // Navegar a detalles (lo implementaremos luego)
        [RelayCommand]
        private async Task GoToDetails(AppAppointment appointment)
        {
            if (appointment == null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                { "AppointmentObj", appointment }
            };

            // Navegamos a la nueva página pasando el objeto
            await Shell.Current.GoToAsync(nameof(AppointmentDetailsPage), navigationParameter);
        }
    }
}