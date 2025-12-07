using Android.App;
using Android.Content;
using Android.Util;
using AndroidX.Core.App;
using Firebase.Messaging;

namespace appointex.Platforms.Android
{
    [Service(Exported = false)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);
            // Tu lógica de token está bien
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);

            // Si la app está abierta, construimos la notificación manualmente
            try
            {
                var notification = message.GetNotification();
                if (notification != null)
                {
                    SendLocalNotification(notification.Title, notification.Body);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("FCM", "Error mostrando notificación: " + ex.Message);
            }
        }

        private void SendLocalNotification(string title, string body)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            
            // PendingIntent para cuando toquen la notificación
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot | PendingIntentFlags.Immutable);

            var channelId = "default"; // El mismo ID que creamos en MainActivity

            var notificationBuilder = new NotificationCompat.Builder(this, channelId)
                .SetSmallIcon(Resource.Mipmap.appicon) // Asegúrate de tener un icono válido aquí
                .SetContentTitle(title)
                .SetContentText(body)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetPriority(NotificationCompat.PriorityHigh);

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(new Random().Next(), notificationBuilder.Build());
        }
    }
}