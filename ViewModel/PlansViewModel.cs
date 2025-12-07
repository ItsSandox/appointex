using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using appointex.Models;
using System.Collections.ObjectModel;

namespace appointex.ViewModels
{
    public partial class PlansViewModel : ObservableObject
    {
        public ObservableCollection<PlanModel> Plans { get; }

        public PlansViewModel()
        {
            Plans = new ObservableCollection<PlanModel>
            {
                // PLAN BÁSICO
                new PlanModel
                {
                    Name = "Básico",
                    Price = "Gratis",
                    Period = "siempre",
                    Description = "Ideal para comenzar a ofrecer tus servicios.",
                    IsPopular = false,
                    HeaderColor = Colors.White,
                    ButtonColor = Color.FromArgb("#f5f5f5"),
                    TextColor = Colors.Black,
                    Features = new ObservableCollection<string>
                    {
                        "Publicar hasta 1 servicio",
                        "Gestión básica de citas",
                        "Notificaciones estándar",
                        "Soporte por correo"
                    }
                },

                // PLAN PRO (Destacado - RD$ 600 aprox $10 USD)
                new PlanModel
                {
                    Name = "Profesional",
                    Price = "RD$ 600", 
                    Period = "/mes",
                    Description = "Potencia tu alcance y gestiona más clientes.",
                    IsPopular = true,
                    HeaderColor = Color.FromArgb("#fff8d6"), 
                    ButtonColor = Color.FromArgb("#ffd917"), 
                    TextColor = Colors.Black,
                    Features = new ObservableCollection<string>
                    {
                        "Publicar hasta 10 servicios",
                        "Agenda avanzada",
                        "Estadísticas de rendimiento",
                        "Notificaciones prioritarias",
                        "Insignia de verificado"
                    }
                },

                // PLAN EMPRESARIAL (RD$ 1,800 aprox $30 USD)
                new PlanModel
                {
                    Name = "Empresarial",
                    Price = "RD$ 1,800",
                    Period = "/mes",
                    Description = "Sin límites. Para negocios en expansión.",
                    IsPopular = false,
                    HeaderColor = Colors.Black,
                    ButtonColor = Colors.White,
                    TextColor = Colors.White,
                    Features = new ObservableCollection<string>
                    {
                        "Servicios ilimitados",
                        "Múltiples colaboradores",
                        "Soporte 24/7 dedicado",
                        "Personalización de marca",
                        "Exportación de datos",
                        "Mayor visibilidad en búsquedas"
                    }
                }
            };
        }

        [RelayCommand]
        private async Task SelectPlan(PlanModel plan)
        {
            // Lógica simulada de selección
            await Shell.Current.DisplayAlert("Plan Seleccionado", $"Has elegido el plan {plan.Name} por {plan.Price}", "Continuar");
            await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
        }

        [RelayCommand]
        private async Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}