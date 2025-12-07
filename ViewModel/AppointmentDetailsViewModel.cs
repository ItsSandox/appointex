using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using appointex.Models;
using appointex.Services;
using System.Collections.ObjectModel;

namespace appointex.ViewModels
{

    [QueryProperty(nameof(Appointment), "AppointmentObj")]
    public partial class AppointmentDetailsViewModel : ObservableObject
    {
        private readonly SupabaseService _supabaseService;

        public AppointmentDetailsViewModel(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
            StatusOptions = new ObservableCollection<StatusOption>();
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StatusColor))] // Para actualizar la vista al cambiar
        [NotifyPropertyChangedFor(nameof(StatusText))]
        private AppAppointment _appointment;

        // --- Propiedades para la Vista ---
        [ObservableProperty] private string _otherPartyName;
        [ObservableProperty] private string _otherPartyRole;
        [ObservableProperty] private string _otherPartyPhoto;

        // Controla si se ven los botones de cambiar estado
        [ObservableProperty] private bool _isPartner;

        // Lista de botones de estado
        public ObservableCollection<StatusOption> StatusOptions { get; }

        // Redirecciones rápidas para que la UI se actualice al instante
        public Color StatusColor => Appointment?.StatusColor ?? Colors.Gray;
        public string StatusText => Appointment?.StatusText ?? "";


        partial void OnAppointmentChanged(AppAppointment value)
        {
            if (value != null)
            {
                DetermineViewRole(value);
            }
        }

        private void DetermineViewRole(AppAppointment appt)
        {
            var myId = _supabaseService.Client.Auth.CurrentUser?.Id;
            if (string.IsNullOrEmpty(myId)) return;

            if (appt.ClientId == myId)
            {
                // SOY CLIENTE
                IsPartner = false;
                OtherPartyName = appt.Service?.User?.Username ?? "Socio";
                OtherPartyRole = "Proveedor del servicio";
                OtherPartyPhoto = appt.Service?.User?.ProfilePhotoUrl ?? "user_placeholder.png";
            }
            else
            {
                // SOY SOCIO (Puedo cambiar estado)
                IsPartner = true;
                OtherPartyName = appt.Client?.Username ?? "Cliente";
                OtherPartyRole = "Cliente solicitante";
                OtherPartyPhoto = appt.Client?.ProfilePhotoUrl ?? "user_placeholder.png";

                LoadStatusOptions();
            }
        }

        private void LoadStatusOptions()
        {
            StatusOptions.Clear();
            // Agregamos los estados posibles para el socio
            StatusOptions.Add(new StatusOption { Id = 2, Text = "Aprobar", Color = Color.FromArgb("#32d583") }); // Verde
            StatusOptions.Add(new StatusOption { Id = 5, Text = "Completar", Color = Color.FromArgb("#32d583") }); // Verde
            StatusOptions.Add(new StatusOption { Id = 3, Text = "Rechazar", Color = Color.FromArgb("#f97066") }); // Rojo
            // StatusOptions.Add(new StatusOption { Id = 4, Text = "Cancelar", Color = Colors.Orange }); // Opcional
        }

        [RelayCommand]
        private async Task ChangeStatus(StatusOption option)
        {
            if (Appointment == null) return;

            bool confirm = await Shell.Current.DisplayAlert("Confirmar", $"¿Cambiar estado a {option.Text}?", "Sí", "No");
            if (!confirm) return;

            try
            {
                // 1. Actualizar en Base de Datos
                await _supabaseService.Client
                    .From<AppAppointment>()
                    .Where(x => x.Id == Appointment.Id)
                    .Set(x => x.State, option.Id)
                    .Update();

                // 2. Actualizar Localmente (para que la UI cambie de color al instante)
                Appointment.State = option.Id;

                // Forzamos la actualización de la UI notificando que 'Appointment' cambió
                OnPropertyChanged(nameof(Appointment));
                OnPropertyChanged(nameof(StatusColor));
                OnPropertyChanged(nameof(StatusText));

                await Shell.Current.DisplayAlert("Actualizado", $"La cita ahora está: {option.Text}", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo actualizar el estado", "OK");
            }
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}