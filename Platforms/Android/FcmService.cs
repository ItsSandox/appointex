using Android.Gms.Extensions;
using Firebase.Messaging;
using appointex.Services;

namespace appointex.Platforms.Android
{
    public class FcmService : IFcmService
    {
        public async Task<string> GetTokenAsync()
        {
            try
            {
                // Usamos la API nativa de Android para obtener el token
                var tokenResult = await FirebaseMessaging.Instance.GetToken();
                return tokenResult.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo token FCM: {ex.Message}");
                return string.Empty;
            }
        }
    }
}