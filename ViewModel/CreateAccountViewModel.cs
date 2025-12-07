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

        // Variable para guardar el archivo de foto temporalmente
        private FileResult? _selectedImageFile;

        public CreateAccountViewModel(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
            // Imagen por defecto (puedes poner una url o recurso local)
            ProfilePhotoDisplay = "subir.png";
        }

        // --- PROPIEDADES ---
        [ObservableProperty] private string _username;
        [ObservableProperty] private string _email;
        [ObservableProperty] private string _password;
        [ObservableProperty] private string _profession;

        // Esta propiedad controla qué imagen se ve en el círculo
        [ObservableProperty] private ImageSource _profilePhotoDisplay;

        // 1 = Cliente, 2 = Socio (Se asignará desde el CodeBehind de la vista)
        [ObservableProperty] private int _role;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        private bool _isBusy;


        // --- COMANDO: SELECCIONAR FOTO (Android/iOS Picker) ---
        [RelayCommand]
        private async Task PickImage()
        {
            try
            {
                // Abre la galería nativa del teléfono
                var result = await MediaPicker.Default.PickPhotoAsync();

                if (result != null)
                {
                    _selectedImageFile = result;
                    // Actualizamos la UI para que el usuario vea la foto que eligió
                    var stream = await result.OpenReadAsync();
                    ProfilePhotoDisplay = ImageSource.FromStream(() => stream);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo seleccionar la imagen.", "OK");
            }
        }


        // --- COMANDO: REGISTRAR ---
        [RelayCommand]
        private async Task Register()
        {
            if (IsBusy) return;
            // Validaciones básicas...
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Username))
            {
                await Shell.Current.DisplayAlert("Faltan datos", "Por favor llena los campos obligatorios.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                // ---------------------------------------------------------
                // PASO 1: CREAR LA CUENTA (SignUp)
                // ---------------------------------------------------------
                var session = await _supabaseService.Client.Auth.SignUp(Email, Password);

                // Verificamos si se creó el usuario
                if (session?.User?.Id == null)
                {
                    // Si falla, intentamos ver si el usuario ya existe o lanzar error
                    throw new Exception("No se pudo crear el usuario. Verifica si el correo ya existe.");
                }

                // ---------------------------------------------------------
                // PASO 2: FORZAR INICIO DE SESIÓN (SignIn) - CRUCIAL PARA RLS
                // ---------------------------------------------------------
                // Esto asegura que el cliente de Supabase tenga el TOKEN de acceso válido
                // para poder escribir en la tabla 'users' y en el 'storage'.
                try
                {
                    await _supabaseService.Client.Auth.SignIn(Email, Password);
                }
                catch (Exception loginEx)
                {
                    // OJO: Si tienes "Confirmar Email" activado en Supabase, esto fallará
                    // porque el usuario aún no ha verificado su correo.
                    throw new Exception("Cuenta creada, pero no se pudo iniciar sesión automáticamente. Posiblemente requieras confirmar tu email.");
                }

                // Recuperamos el ID del usuario YA AUTENTICADO
                var currentUser = _supabaseService.Client.Auth.CurrentUser;
                if (currentUser == null) throw new Exception("Error obteniendo la sesión del usuario.");

                string userId = currentUser.Id;
                string photoUrl = null;

                // ---------------------------------------------------------
                // PASO 3: SUBIR FOTO (Ahora sí tenemos permiso RLS)
                // ---------------------------------------------------------
                if (_selectedImageFile != null)
                {
                    var fileName = $"{userId}{Path.GetExtension(_selectedImageFile.FileName)}";

                    using var stream = await _selectedImageFile.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();

                    await _supabaseService.Client.Storage
                        .From("profile_pictures") // Asegúrate que este bucket sea público o tenga policies correctas
                        .Upload(imageBytes, fileName);

                    photoUrl = _supabaseService.Client.Storage
                        .From("profile_pictures")
                        .GetPublicUrl(fileName);
                }

                // ---------------------------------------------------------
                // PASO 4: INSERTAR DATOS EN TABLA PÚBLICA (Ahora sí tenemos permiso RLS)
                // ---------------------------------------------------------
                var newUser = new AppUser
                {
                    UserId = userId, // Este ID debe coincidir con auth.uid() en tus políticas RLS
                    Username = Username,
                    Role = Role,
                    Profession = Role == 2 ? Profession : null,
                    ProfilePhotoUrl = photoUrl
                };
                await Shell.Current.DisplayAlert("Error", $"Detalle: {userId}", "OK");

                await _supabaseService.Client.From<AppUser>().Insert(newUser);

                // ---------------------------------------------------------
                // PASO 5: NAVEGACIÓN
                // ---------------------------------------------------------
                await Shell.Current.DisplayAlert("¡Bienvenido!", "Tu cuenta ha sido configurada.", "OK");
                await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");

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