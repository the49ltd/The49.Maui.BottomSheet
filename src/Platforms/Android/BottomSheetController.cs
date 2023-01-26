using Android.Views;
using Microsoft.Maui.Platform;
using Google.Android.Material.BottomSheet;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.DrawerLayout.Widget;
using Android.Content.Res;

namespace The49.Maui.BottomSheet;

public class BottomSheetController : IBottomSheetController
{
    BottomSheetBehavior _behavior = new BottomSheetBehavior
    {
        State = BottomSheetBehavior.StateHidden,
    };
    public BottomSheetBehavior Behavior => _behavior;

    CoordinatorLayout _coordinatorLayout;
    ViewGroup _frame;

    IMauiContext _windowMauiContext { get; }
    BottomSheetPage _page { get; }

    public BottomSheetController(IMauiContext windowMauiContext, BottomSheetPage page)
    {
        _windowMauiContext = windowMauiContext;
        _page = page;
    }

    public void Dismiss()
    {
        Behavior.Hideable = true;
        Behavior.State = BottomSheetBehavior.StateHidden;
    }

    void Dispose()
    {
        _coordinatorLayout.RemoveFromParent();
    }

    public void Layout()
    {
        // TODO: verify that, maybe handle statusbar and navigationbar
        var maxSheetHeight = _page.Window.Height;
        BottomSheetManager.LayoutDetents(_behavior, _frame, _page, maxSheetHeight);
    }

    public void UpdateBackground()
    {
        Paint paint = _page.BackgroundBrush;
        if (_frame != null && paint != null)
        {
            _frame.BackgroundTintList = ColorStateList.ValueOf(paint.ToColor().ToPlatform());
        }
    }

    public void Show()
    {
        var navigationRootManager = _windowMauiContext.Services.GetRequiredService<NavigationRootManager>();
        ViewGroup view = null;

        if (navigationRootManager.RootView is Microsoft.Maui.Platform.ContainerView cv && cv.MainView is DrawerLayout drawerLayout)
        {
            view = drawerLayout;
        }
        else if (navigationRootManager.RootView is CoordinatorLayout coordinatorLayout)
        {
            view = coordinatorLayout;
        }
        else
        {
            throw new Exception("Unrecognized RootView");
        }

        var li = LayoutInflater.From(_windowMauiContext.Context);
        _coordinatorLayout = (CoordinatorLayout)li.Inflate(Resource.Layout.custom_bottom_sheet, null);
        var layout = BottomSheetManager.CreateLayout(_page, _windowMauiContext);
        _frame = _coordinatorLayout.FindViewById<FrameLayout>(Resource.Id.design_bottom_sheet);
        _frame.AddView(layout);
        view.AddView(_coordinatorLayout, new ViewGroup.LayoutParams(DrawerLayout.LayoutParams.MatchParent, DrawerLayout.LayoutParams.MatchParent));


        _behavior = (BottomSheetBehavior)((CoordinatorLayout.LayoutParams)_frame.LayoutParameters).Behavior;
        _behavior.State = BottomSheetBehavior.StateHidden;

        var callback = new BottomSheetPageCallback(_page);
        callback.StateChanged += Callback_StateChanged;
        Behavior.AddBottomSheetCallback(callback);
        _page.Dispatcher.Dispatch(() =>
        {
            UpdateBackground();
            Layout();

            Behavior.State = Behavior.SkipCollapsed ? BottomSheetBehavior.StateExpanded : BottomSheetBehavior.StateCollapsed;

            _behavior.Hideable = _page.Cancelable;
        });
    }

    private void Callback_StateChanged(object sender, EventArgs e)
    {
        if (Behavior.State == BottomSheetBehavior.StateHidden)
        {
            Dispose();
        }
    }
}
