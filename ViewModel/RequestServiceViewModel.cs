using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using appointex.Models;
using appointex.Services;
using System.Collections.ObjectModel;

namespace appointex.ViewModels
{
    [QueryProperty(nameof(Service), "ServiceObj")]
    public partial class RequestServiceViewModel : ObservableObject
    {
        private readonly SupabaseService _supabaseService;

        public RequestServiceViewModel(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
            // Inicializamos fechas por defecto (hoy)
            SelectedDate = DateTime.Now;
            SelectedTime = DateTime.Now.TimeOfDay;
        }

        // --- OBJETO RECIBIDO (El servicio que se va a reservar) ---
        [ObservableProperty]
        private AppService _service;

        // --- DATOS DEL FORMULARIO DE RESERVA ---
        [ObservableProperty] private DateTime _selectedDate;
        [ObservableProperty] private TimeSpan _selectedTime;
        [ObservableProperty] private string _note;
        
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RequestCommand))]
        private bool _isBusy;

        // --- PROPIEDADES DE SOLO LECTURA PARA LA VISTA ---
        public string ServiceName => Service?.Name;
        public string ServiceDescription => Service?.Description;
        public float ServicePrice => Service?.Price ?? 0;
        public TimeSpan ServiceDuration => Service?.Duration ?? TimeSpan.Zero;
        public string PartnerName => Service?.User?.Username ?? "Socio";
        public string PartnerPhoto => Service?.User?.ProfilePhotoUrl ?? "user_placeholder.png";
        public string PartnerProfession => !string.IsNullOrEmpty(Service?.User?.Profession) 
                                       ? Service.User.Profession 
                                       : "Socio Profesional";
        
        public ObservableCollection<string> ServiceImages { get; set; } = new();

        partial void OnServiceChanged(AppService value)
        {
            if (value != null)
            {
                OnPropertyChanged(nameof(ServiceName));
                OnPropertyChanged(nameof(ServiceDescription));
                OnPropertyChanged(nameof(ServicePrice));
                OnPropertyChanged(nameof(ServiceDuration));
                OnPropertyChanged(nameof(PartnerName));
                OnPropertyChanged(nameof(PartnerPhoto));
                OnPropertyChanged(nameof(PartnerProfession));

                ServiceImages.Clear();
                if (!string.IsNullOrEmpty(value.ServicePhoto))
                    ServiceImages.Add(value.ServicePhoto);
                else
                    ServiceImages.Add("limpia.png"); 
            }
        }

        [RelayCommand]
        private async Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }

        // --- LÓGICA DE CREAR CITA ---
        [RelayCommand]
        private async Task Request()
        {
            if (IsBusy) return;

            // 1. Validaciones
            if (SelectedDate.Date < DateTime.Now.Date)
            {
                await Shell.Current.DisplayAlert("Fecha inválida", "No puedes reservar en el pasado.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                var currentUser = _supabaseService.Client.Auth.CurrentUser;
                if (currentUser == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Debes iniciar sesión.", "OK");
                    return;
                }

                // 2. Combinar Fecha y Hora
                DateTime finalDateTime = SelectedDate.Date + SelectedTime;

                // 3. Crear objeto Appointment
                var newAppointment = new AppAppointment
                {
                    ServiceId = Service.Id,
                    ClientId = currentUser.Id,
                    DateTime = finalDateTime, // Se guarda como timestampz
                    Note = Note,
                    State = 1 // 1 = Pendiente (Color fec84b)
                };

                // 4. Insertar en Supabase
                await _supabaseService.Client.From<AppAppointment>().Insert(newAppointment);

                await Shell.Current.DisplayAlert("Solicitud Enviada", "El socio ha recibido tu solicitud pendiente de aprobación.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}