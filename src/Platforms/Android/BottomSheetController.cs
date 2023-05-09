using Android.Views;
using Microsoft.Maui.Platform;
using Google.Android.Material.BottomSheet;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.DrawerLayout.Widget;
using Android.Content.Res;
using Google.Android.Material.Internal;
using AView = Android.Views.View;
using AndroidX.Core.View;
using Android.Graphics.Drawables;
using Google.Android.Material.Shape;
using Microsoft.Maui.Controls;
using Google.Android.Material.Color;

namespace The49.Maui.BottomSheet;

public class BottomSheetController : IBottomSheetController
{
    internal class EdgeToEdgeCallback : BottomSheetBehavior.BottomSheetCallback
    {
        WindowInsetsCompat _insetsCompat;
        private BottomSheetController _controller;
        private bool? _lightBottomSheet;

        internal EdgeToEdgeCallback(BottomSheetController controller, WindowInsetsCompat insetsCompat) : base()
        {
            _insetsCompat = insetsCompat;
            _controller = controller;

            var backgroundTint = ViewCompat.GetBackgroundTintList(controller._frame);

            if (backgroundTint != null)
            {
                // First check for a tint
                _lightBottomSheet = MaterialColors.IsColorLight(backgroundTint.DefaultColor);
            }
            else if (controller._frame.Background is ColorDrawable colorDrawable)
            {
                // Then check for the background color
                _lightBottomSheet = MaterialColors.IsColorLight(colorDrawable.Color);
            }
            else
            {
                // Otherwise don't change the status bar color
                _lightBottomSheet = null;
            }
        }
        public override void OnSlide(AView bottomSheet, float newState)
        {
            SetPaddingForPosition(bottomSheet);
        }

        public override void OnStateChanged(AView bottomSheet, int p1)
        {
            SetPaddingForPosition(bottomSheet);
        }
        private void SetPaddingForPosition(AView bottomSheet)
        {
            var window = ((Android.App.Activity)_controller._sheet.Window.Handler.PlatformView).Window;

            var insetsController =
            WindowCompat.GetInsetsController(window, window.DecorView);
            var lightStatusBar = insetsController.AppearanceLightStatusBars;

            if (bottomSheet.Top < _insetsCompat.SystemWindowInsetTop)
            {
                // If the bottomsheet is light, we should set light status bar so the icons are visible
                // since the bottomsheet is now under the status bar.
                if (window != null)
                {
                    EdgeToEdgeUtils.SetLightStatusBar(
                        window, !_lightBottomSheet.HasValue ? lightStatusBar : _lightBottomSheet.Value);
                }
                // Smooth transition into status bar when drawing edge to edge.
                bottomSheet.SetPadding(
                    bottomSheet.PaddingLeft,
                    (_insetsCompat.SystemWindowInsetTop - bottomSheet.Top),
                    bottomSheet.PaddingRight,
                    bottomSheet.PaddingBottom);
            }
            else if (bottomSheet.Top != 0)
            {
                // Reset the status bar icons to the original color because the bottomsheet is not under the
                // status bar.
                if (window != null)
                {
                    EdgeToEdgeUtils.SetLightStatusBar(window, lightStatusBar);
                }
                bottomSheet.SetPadding(
                    bottomSheet.PaddingLeft,
                    0,
                    bottomSheet.PaddingRight,
                    bottomSheet.PaddingBottom);
            }
        }
    }
    internal class OnApplyWindowInsetsListener : Java.Lang.Object, IOnApplyWindowInsetsListener
    {
        private BottomSheetController _controller;

        internal OnApplyWindowInsetsListener(BottomSheetController controller) : base()
        {
            _controller = controller;
        }
        public WindowInsetsCompat OnApplyWindowInsets(AView v, WindowInsetsCompat insets)
        {
            if (_controller._edgeToEdgeCallback != null)
            {
                _controller.Behavior.RemoveBottomSheetCallback(_controller._edgeToEdgeCallback);
            }

            if (insets != null)
            {
                _controller._edgeToEdgeCallback = new EdgeToEdgeCallback(_controller, insets);
                _controller.Behavior.AddBottomSheetCallback(_controller._edgeToEdgeCallback);
            }
            return insets;
        }
    }
    BottomSheetBehavior.BottomSheetCallback _edgeToEdgeCallback { get; set; }

    BottomSheetBehavior _behavior = new BottomSheetBehavior
    {
        State = BottomSheetBehavior.StateHidden,
    };
    public BottomSheetBehavior Behavior => _behavior;

    CoordinatorLayout _coordinatorLayout;
    ViewGroup _frame;

    IMauiContext _windowMauiContext { get; }
    BottomSheet _sheet { get; }

    public BottomSheetController(IMauiContext windowMauiContext, BottomSheet sheet)
    {
        _windowMauiContext = windowMauiContext;
        _sheet = sheet;
    }

    public void Dismiss()
    {
        Behavior.Hideable = true;
        Behavior.State = BottomSheetBehavior.StateHidden;
    }

    void Dispose()
    {
        var navigationRootManager = _windowMauiContext.Services.GetRequiredService<NavigationRootManager>();
        if (navigationRootManager.RootView is CoordinatorLayout coordinatorLayout && _coordinatorLayout == coordinatorLayout)
        {
            //_frame.RemoveFromParent();
        }
        else
        {
            _coordinatorLayout.RemoveFromParent();
        }
        _frame = null;
        _coordinatorLayout = null;
    }

    public void Layout()
    {
        // TODO: verify that, maybe handle statusbar and navigationbar
        var maxSheetHeight = _sheet.Window.Height;
        BottomSheetManager.LayoutDetents(_behavior, _frame, _sheet, maxSheetHeight);
    }

    public void UpdateBackground()
    {
        Paint paint = _sheet.BackgroundBrush;
        if (_frame != null && paint != null)
        {
            _frame.BackgroundTintList = ColorStateList.ValueOf(paint.ToColor().ToPlatform());
        }
    }

    void SetupCoordinatorLayout()
    {
        var navigationRootManager = _windowMauiContext.Services.GetRequiredService<NavigationRootManager>();

        if (navigationRootManager.RootView is ContainerView cv && cv.MainView is DrawerLayout drawerLayout)
        {
            _coordinatorLayout = new CoordinatorLayout(_windowMauiContext.Context);
            _coordinatorLayout.SetFitsSystemWindows(true);

            drawerLayout.AddView(_coordinatorLayout, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        }
        else if (navigationRootManager.RootView is CoordinatorLayout coordinatorLayout)
        {
            _coordinatorLayout = coordinatorLayout;
        }
        else
        {
            throw new Exception("Unrecognized RootView");
        }

        _frame = new FrameLayout(new ContextThemeWrapper(_windowMauiContext.Context, Resource.Style.Widget_Material3_BottomSheet_Modal), null, 0);

        _behavior = new BottomSheetBehavior(new ContextThemeWrapper(_windowMauiContext.Context, Resource.Style.Widget_Material3_BottomSheet_Modal), null);
        _behavior.State = BottomSheetBehavior.StateHidden;

        _coordinatorLayout.AddView(_frame, new CoordinatorLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
        {
            Gravity = (int)(GravityFlags.CenterHorizontal | GravityFlags.Top),
            Behavior = _behavior,
        });

        ViewCompat.SetOnApplyWindowInsetsListener(_frame, new OnApplyWindowInsetsListener(this));
    }

    public void Show()
    {
        SetupCoordinatorLayout();

        var layout = BottomSheetManager.CreateLayout(_sheet, _windowMauiContext);

        layout.LayoutChange += (s, e) => Layout();

        _frame.AddView(layout);

        var callback = new BottomSheetCallback(_sheet);
        callback.StateChanged += Callback_StateChanged;
        Behavior.AddBottomSheetCallback(callback);
        _sheet.Dispatcher.Dispatch(() =>
        {
            UpdateBackground();
            Layout();

            Behavior.State = Behavior.SkipCollapsed ? BottomSheetBehavior.StateExpanded : BottomSheetBehavior.StateCollapsed;

            _behavior.Hideable = _sheet.IsCancelable;

            _sheet.NotifyShowing();
        });
    }

    private void Callback_StateChanged(object sender, EventArgs e)
    {
        if (Behavior.State == BottomSheetBehavior.StateHidden)
        {
            _sheet.NotifyDismissed();
            Dispose();
        }
    }
}
