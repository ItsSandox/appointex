using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace appointex.ViewModels
{
    // Heredamos de ObservableObject para la notificación de cambios automática
    public partial class InicioViewModel : ObservableObject
    {
        // Aquí definiríamos los servicios en el futuro
        // private readonly IAuthService _authService;

        // Constructor: Aquí inyectarás tus servicios cuando el proyecto escale
        public InicioViewModel()
        {
            // _authService = authService;
            
            // Inicializamos la vista en estado "Bienvenida"
            SetWelcomeState();
        }

        // --- PROPIEDADES OBSERVABLES (BINDINGS) ---
        // Al cambiar estas variables, la UI se actualiza sola gracias a [ObservableProperty]

        [ObservableProperty]
        private string _titleText;

        [ObservableProperty]
        private string _subtitleText;

        [ObservableProperty]
        private string _primaryButtonText;

        [ObservableProperty]
        private string _secondaryButtonText;

        // Propiedad para saber qué acción ejecutar (Login o Registro real)
        [ObservableProperty]
        private bool _isLoginMode; 
        
        [ObservableProperty]
        private bool _isBackButtonVisible;


        // --- COMANDOS (ACCIONES DE LOS BOTONES) ---

        [RelayCommand]
        private async Task PrimaryAction()
        {
            if (_titleText == "Bienvenido")
            {
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            else
            {
                await Shell.Current.GoToAsync(nameof(CreateAccountClientPage));
            }

        }

        [RelayCommand]
        private async Task SecondaryAction()
        {
            // El botón secundario actúa como "Crear Cuenta" al inicio o "Cancelar/Volver" después
            if (_titleText == "Bienvenido")
            {
                SetRegisterState();
            }
            else
            {
                await Shell.Current.GoToAsync(nameof(CreateAccountPartnerPage));
            }
        }

        // --- MÉTODOS DE ESTADO (Lógica de UI) ---
         [RelayCommand]
        private void BackAction()
        {
            SetWelcomeState();
        }
        private void SetWelcomeState()
        {
            TitleText = "Bienvenido";
            SubtitleText = "Puedes iniciar sesión si ya tienes cuenta o crear una nueva en segundos.";
            PrimaryButtonText = "Iniciar Sesión";
            SecondaryButtonText = "Crear Cuenta";
            IsLoginMode = false;
            IsBackButtonVisible = false; 
        }

        private void SetRegisterState()
        {
            TitleText = "¿Cómo quieres continuar?";
            SubtitleText = "Escoge si usarás la aplicación como cliente o como socio.";
            PrimaryButtonText = "Cliente";
            SecondaryButtonText = "Socio";
            IsLoginMode = false;
            IsBackButtonVisible = true;
        }
    }
}