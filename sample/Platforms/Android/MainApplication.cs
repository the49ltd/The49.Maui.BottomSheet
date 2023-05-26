using Android.App;
using Android.Runtime;
using Google.Android.Material.Color;

namespace The49.Maui.BottomSheet.Sample;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

}
