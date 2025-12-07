using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using appointex.Services;
using appointex.Models;
using FileResult = Microsoft.Maui.Storage.FileResult;

namespace appointex.ViewModels
{
    public partial class CreateAccountViewModel : ObservableObject
    {
        private readonly SupabaseService _supabaseService;
        private FileResult? _selectedImageFile;

        public CreateAccountViewModel(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
            ProfilePhotoDisplay = "subir.png";
        }

        [ObservableProperty] private string _username;
        [ObservableProperty] private string _email;
        [ObservableProperty] private string _password;
        [ObservableProperty] private string _profession;
        [ObservableProperty] private ImageSource _profilePhotoDisplay;
        [ObservableProperty] private int _role; // 1 = Cliente, 2 = Socio
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        private bool _isBusy;

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
                    ProfilePhotoDisplay = ImageSource.FromStream(() => stream);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo seleccionar la imagen.", "OK");
            }
        }

        [RelayCommand]
        private async Task Register()
        {
            if (IsBusy) return;
            
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Username))
            {
                await Shell.Current.DisplayAlert("Faltan datos", "Por favor llena los campos obligatorios.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                // PASO 1: CREAR CUENTA
                var session = await _supabaseService.Client.Auth.SignUp(Email, Password);

                if (session?.User?.Id == null)
                    throw new Exception("No se pudo crear el usuario. Verifica si el correo ya existe.");

                // PASO 2: INICIAR SESIÓN (Necesario para RLS)
                try
                {
                    await _supabaseService.Client.Auth.SignIn(Email, Password);
                }
                catch (Exception)
                {
                    throw new Exception("Cuenta creada, pero error al iniciar sesión.");
                }

                var currentUser = _supabaseService.Client.Auth.CurrentUser;
                if (currentUser == null) throw new Exception("Error obteniendo sesión.");

                string userId = currentUser.Id;
                string photoUrl = null;

                // PASO 3: SUBIR FOTO
                if (_selectedImageFile != null)
                {
                    var fileName = $"{userId}{Path.GetExtension(_selectedImageFile.FileName)}";
                    using var stream = await _selectedImageFile.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();

                    await _supabaseService.Client.Storage
                        .From("profile_pictures")
                        .Upload(imageBytes, fileName);

                    photoUrl = _supabaseService.Client.Storage
                        .From("profile_pictures")
                        .GetPublicUrl(fileName);
                }

                // PASO 4: INSERTAR EN BASE DE DATOS
                var newUser = new AppUser
                {
                    UserId = userId,
                    Username = Username,
                    Role = Role,
                    Profession = Role == 2 ? Profession : null,
                    ProfilePhotoUrl = photoUrl
                };

                await _supabaseService.Client.From<AppUser>().Insert(newUser);

                // ---------------------------------------------------------
                // PASO 5: NAVEGACIÓN CONDICIONAL
                // ---------------------------------------------------------
                await Shell.Current.DisplayAlert("¡Bienvenido!", "Tu cuenta ha sido creada con éxito.", "OK");

                if (Role == 2)
                {
                    // ES SOCIO -> Mandar a PlansPage
                    // Asegúrate de tener: Routing.RegisterRoute(nameof(PlansPage), typeof(PlansPage)); en AppShell
                    await Shell.Current.GoToAsync(nameof(PlansPage));
                }
                else
                {
                    // ES USUARIO -> Mandar a Dashboard
                    // Usamos // para reiniciar la pila de navegación
                    await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Detalle: {ex.Message}", "OK");
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