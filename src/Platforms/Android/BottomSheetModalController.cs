using Android.App;
using Android.OS;
using Android.Views;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;
using Google.Android.Material.BottomSheet;
using AndroidX.AppCompat.App;
using Microsoft.Maui.Controls;
using Org.Apache.Commons.Logging;
using Android.Widget;
using Android.Content.Res;

namespace The49.Maui.BottomSheet;

public class BottomSheetModalController : BottomSheetDialogFragment, IBottomSheetController
{
    readonly BottomSheetPage _page;
    readonly IMauiContext _mauiWindowContext;
    NavigationRootManager? _navigationRootManager;
    BottomSheetBehavior _behavior;

    public NavigationRootManager? NavigationRootManager
    {
        get => _navigationRootManager;
        private set => _navigationRootManager = value;
    }

    public BottomSheetBehavior Behavior => _behavior;

    public BottomSheetModalController(IMauiContext mauiContext, BottomSheetPage page)
    {
        _navigationRootManager = mauiContext.Services.GetRequiredService<NavigationRootManager>();
        _page = page;
        _mauiWindowContext = mauiContext;
    }

    public override AView OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        return BottomSheetManager.CreateLayout(_page, _mauiWindowContext);
    }

    public override Dialog OnCreateDialog(Bundle savedInstanceState)
    {
        var dialog = base.OnCreateDialog(savedInstanceState);

        var d = (BottomSheetDialog)dialog;

       _behavior = d.Behavior;

        Cancelable = _page.Cancelable;
        d.Behavior.Hideable = _page.Cancelable;
        d.Behavior.AddBottomSheetCallback(new BottomSheetPageCallback(_page));

        d.ShowEvent += (s, e) =>
        {
            UpdateBackground();
            Layout();
            Behavior.State = Behavior.SkipCollapsed ? BottomSheetBehavior.StateExpanded : BottomSheetBehavior.StateCollapsed;
        };

        return dialog;
    }

    public void UpdateBackground()
    {
        var frame = (FrameLayout)View?.Parent;
        Paint paint = _page.BackgroundBrush;
        if (frame != null && paint != null)
        {
            frame.BackgroundTintList = ColorStateList.ValueOf(paint.ToColor().ToPlatform());
        }
    }

    public void Show()
    {
        Show(((AppCompatActivity)Platform.CurrentActivity).SupportFragmentManager, nameof(BottomSheetModalController));
    }

    public void Layout()
    {
        BottomSheetManager.LayoutDetents(Behavior, (ViewGroup)View, _page, _page.Window.Height);
    }
}
