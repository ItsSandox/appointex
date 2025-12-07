using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using appointex.Models;
using appointex.Services;
using FileResult = Microsoft.Maui.Storage.FileResult;

namespace appointex.ViewModels
{
    [QueryProperty(nameof(Service), "ServiceObj")]
    public partial class EditServiceViewModel : ObservableObject
    {
        private readonly SupabaseService _supabaseService;
        private FileResult? _selectedImageFile;

        public EditServiceViewModel(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        // El servicio original que llega desde el Dashboard
        [ObservableProperty]
        private AppService _service;

        // Propiedades editables
        [ObservableProperty] private string _name;
        [ObservableProperty] private string _description;
        [ObservableProperty] private float _price;
        [ObservableProperty] private TimeSpan _duration;
        [ObservableProperty] private ImageSource _serviceImageDisplay;
        
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveChangesCommand))]
        private bool _isBusy;

        // Al recibir el servicio, llenamos los campos
        partial void OnServiceChanged(AppService value)
        {
            if (value != null)
            {
                Name = value.Name;
                Description = value.Description;
                Price = value.Price;
                Duration = value.Duration;
                
                if (!string.IsNullOrEmpty(value.ServicePhoto))
                    ServiceImageDisplay = value.ServicePhoto;
                else
                    ServiceImageDisplay = "limpia.png";
            }
        }

        [RelayCommand]
        private async Task ChangeImage()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync();
                if (result != null)
                {
                    _selectedImageFile = result;
                    var stream = await result.OpenReadAsync();
                    ServiceImageDisplay = ImageSource.FromStream(() => stream);
                }
            }
            catch (Exception) { }
        }

        [RelayCommand]
        private async Task SaveChanges()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                string photoUrl = Service.ServicePhoto; // Mantenemos la anterior por defecto

                // 1. Si eligió foto nueva, la subimos
                if (_selectedImageFile != null)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(_selectedImageFile.FileName)}";
                    using var stream = await _selectedImageFile.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();

                    await _supabaseService.Client.Storage
                        .From("service_pictures")
                        .Upload(imageBytes, fileName);

                    photoUrl = _supabaseService.Client.Storage
                        .From("service_pictures")
                        .GetPublicUrl(fileName);
                }

                // 2. Actualizamos el objeto (usamos Postgrest.QueryOptions para seguridad opcional)
                var updatedService = new AppService
                {
                    Id = Service.Id, // IMPORTANTE: El mismo ID
                    UserId = Service.UserId, // Mismo dueño
                    Name = Name,
                    Description = Description,
                    Price = Price,
                    Duration = Duration,
                    ServicePhoto = photoUrl
                };

                await _supabaseService.Client
                    .From<AppService>()
                    .Update(updatedService);

                await Shell.Current.DisplayAlert("Éxito", "Servicio actualizado correctamente.", "OK");
                await Shell.Current.GoToAsync(".."); // Volver al dashboard
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo actualizar: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}