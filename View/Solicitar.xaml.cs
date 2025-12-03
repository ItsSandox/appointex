using appointex.ViewModel;

namespace appointex.View
{
    public partial class Solicitar : ContentPage
    {
        public Solicitar()
        {
            InitializeComponent();

            // Creamos una nueva instancia del ViewModel y lo asignamos a la página
            this.BindingContext = new SolicitarViewModel();
        }
    }
}