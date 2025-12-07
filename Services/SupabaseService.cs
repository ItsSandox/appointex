using Supabase;
using appointex.Config;
using Supabase.Interfaces; // Necesario para ISupabaseSessionHandler si decides usar persistencia personalizada

namespace appointex.Services
{
    public class SupabaseService
    {
        private readonly Client _client;

        public SupabaseService()
        {
            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
                // SessionHandler = new SupabaseSessionHandler() // Descomenta esto si implementas persistencia en disco
            };

            _client = new Client(ApiKeys.SupabaseUrl, ApiKeys.SupabaseKey, options);
        }

        // Este método se debe llamar UNA sola vez en el arranque (App.xaml.cs)
        public async Task InitializeAsync()
        {
            await _client.InitializeAsync();
        }

        public Client Client => _client;

        // Helper para saber si hay sesión activa
        public bool IsAuthenticated => _client.Auth.CurrentSession != null;
    }
}