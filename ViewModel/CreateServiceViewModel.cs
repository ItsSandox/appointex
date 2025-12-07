using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using appointex.Models;
using appointex.Services;

namespace appointex.ViewModels
{
    public partial class CreateServiceViewModel : ObservableObject
    {
        private readonly SupabaseService _supabaseService;
        private FileResult? _selectedImageFile;

        public CreateServiceViewModel(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
            ServicePhotoDisplay = "subir.png"; // Imagen por defecto
        }

        // --- Propiedades del Formulario ---
        [ObservableProperty] private string _name;
        [ObservableProperty] private string _description;
        [ObservableProperty] private float _price;
        [ObservableProperty] private TimeSpan _duration; // Mapea directo al tipo 'time' de SQL
        
        [ObservableProperty] private ImageSource _servicePhotoDisplay;
        [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(CreateServiceCommand))] private bool _isBusy;

        // --- COMANDO: SELECCIONAR FOTO ---
        [RelayCommand]
        private async Task PickImage()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync();
                if (result != null)
                {
                    _selectedImageFile = result;
                    var stream = await result.OpenReadAsync();
                    ServicePhotoDisplay = ImageSource.FromStream(() => stream);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo cargar la imagen", "OK");
            }
        }

        // --- COMANDO: CREAR SERVICIO ---
        [RelayCommand]
        private async Task CreateService()
        {
            if (IsBusy) return;
            
            // Validaciones
            if (string.IsNullOrWhiteSpace(Name) || Price <= 0)
            {
                await Shell.Current.DisplayAlert("Atención", "Debes poner un nombre y un precio válido.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                // 1. Obtener ID del Usuario Actual
                var currentUser = _supabaseService.Client.Auth.CurrentUser;
                if (currentUser == null) 
                {
                    await Shell.Current.DisplayAlert("Error", "No hay sesión activa.", "OK");
                    return;
                }

                string photoUrl = null;

                // 2. Subir Foto (Si existe)
                if (_selectedImageFile != null)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(_selectedImageFile.FileName)}";
                    
                    using var stream = await _selectedImageFile.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();

                    // Subir al bucket 'service_pictures'
                    await _supabaseService.Client.Storage
                        .From("service_pictures")
                        .Upload(imageBytes, fileName);

                    photoUrl = _supabaseService.Client.Storage
                        .From("service_pictures")
                        .GetPublicUrl(fileName);
                }

                // 3. Crear Objeto Servicio
                var newService = new AppService
                {
                    UserId = currentUser.Id, // Vinculamos con el socio actual
                    Name = Name,
                    Description = Description,
                    Price = Price,
                    Duration = Duration,
                    ServicePhoto = photoUrl
                };

                // 4. Insertar en Base de Datos
                await _supabaseService.Client.From<AppService>().Insert(newService);

                await Shell.Current.DisplayAlert("Éxito", "Servicio publicado correctamente", "OK");
                await Shell.Current.GoToAsync(".."); // Volver al Dashboard
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

        [RelayCommand]
        private async Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}