using Android.App;
using Android.OS;
using Android.Views;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;
using Google.Android.Material.BottomSheet;
using AndroidX.AppCompat.App;
using Android.Widget;
using Android.Content.Res;

namespace The49.Maui.BottomSheet;

public class BottomSheetModalController : BottomSheetDialogFragment, IBottomSheetController
{
    readonly BottomSheet _sheet;
    readonly IMauiContext _mauiWindowContext;
    NavigationRootManager? _navigationRootManager;
    BottomSheetBehavior _behavior;

    public NavigationRootManager? NavigationRootManager
    {
        get => _navigationRootManager;
        private set => _navigationRootManager = value;
    }

    public BottomSheetBehavior Behavior => _behavior;

    public BottomSheetModalController(IMauiContext mauiContext, BottomSheet page)
    {
        _navigationRootManager = mauiContext.Services.GetRequiredService<NavigationRootManager>();
        _sheet = page;
        _mauiWindowContext = mauiContext;
    }

    public override AView OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var layout = BottomSheetManager.CreateLayout(_sheet, _mauiWindowContext);
        layout.LayoutChange += (s, e) => Layout();
        return layout;
    }

    public override Dialog OnCreateDialog(Bundle savedInstanceState)
    {
        var dialog = base.OnCreateDialog(savedInstanceState);

        var d = (BottomSheetDialog)dialog;

       _behavior = d.Behavior;

        Cancelable = _sheet.Cancelable;
        d.Behavior.Hideable = _sheet.Cancelable;
        var callback = new BottomSheetCallback(_sheet);
        callback.StateChanged += OnStateChanged;
        d.Behavior.AddBottomSheetCallback(callback);

        d.ShowEvent += (s, e) =>
        {
            UpdateBackground();
            Layout();
            Behavior.State = Behavior.SkipCollapsed ? BottomSheetBehavior.StateExpanded : BottomSheetBehavior.StateCollapsed;
        };

        return dialog;
    }

    void OnStateChanged(object sender, EventArgs e)
    {
        if (Behavior?.State == BottomSheetBehavior.StateHidden)
        {
            _sheet.NotifyDismissed();
        }
    }

    public void UpdateBackground()
    {
        var frame = (FrameLayout)View?.Parent;
        Paint paint = _sheet.BackgroundBrush;
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
        BottomSheetManager.LayoutDetents(Behavior, (ViewGroup)View, _sheet, _sheet.Window.Height);
    }
}
