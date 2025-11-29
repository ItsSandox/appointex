using Android.App;
using Android.Runtime;
using Android.Content.Res;

namespace appointex;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
    {
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
        {
            if (view is Entry)
            {
                // Quitar linea de los entrys
                handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            }
        });
    }

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

}
