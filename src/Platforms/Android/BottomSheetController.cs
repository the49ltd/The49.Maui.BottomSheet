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
using Google.Android.Material.Color;
using Exception = System.Exception;
using Microsoft.Maui.Controls;

namespace The49.Maui.BottomSheet;

public class BottomSheetController
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
    public IDictionary<Detent, int> _states;
    public IDictionary<Detent, double> _heights;

    bool _isDuringShowingAnimation = false;

    BottomSheetBehavior _behavior;
    public BottomSheetBehavior Behavior => _behavior;

    CoordinatorLayout _coordinatorLayout;
    ViewGroup _frame;
    ViewGroup _layout;
    BottomSheetBackdrop _backdrop;

    IMauiContext _windowMauiContext { get; }
    BottomSheet _sheet { get; }

    public BottomSheetController(IMauiContext windowMauiContext, BottomSheet sheet)
    {
        _windowMauiContext = windowMauiContext;
        _sheet = sheet;
    }

    internal void CalculateHeights(BottomSheet page, double maxSheetHeight)
    {
        var detents = page.GetEnabledDetents().ToList();

        _heights = new Dictionary<Detent, double>();

        if (detents.Count == 0)
        {
            detents = new List<Detent> { new ContentDetent() };
        }

        foreach (var detent in detents)
        {
            _heights.Add(detent, detent.GetHeight(page, maxSheetHeight));
        }
    }

    internal void CalculateStates()
    {
        var heights = _heights.OrderByDescending(kv => kv.Value).ToList();

        _states = new Dictionary<Detent, int>();

        if (heights.Count == 1)
        {
            _states.Add(heights[0].Key, BottomSheetBehavior.StateCollapsed);
        }
        else if (heights.Count == 2)
        {
            _states.Add(heights[0].Key, BottomSheetBehavior.StateExpanded);
            _states.Add(heights[1].Key, BottomSheetBehavior.StateCollapsed);
        }
        else if (heights.Count == 3)
        {
            _states.Add(heights[0].Key, BottomSheetBehavior.StateExpanded);
            _states.Add(heights[1].Key, BottomSheetBehavior.StateHalfExpanded);
            _states.Add(heights[2].Key, BottomSheetBehavior.StateCollapsed);
        }
    }

    internal int GetStateForDetent(Detent detent)
    {
        if (detent is null || !_states.ContainsKey(detent))
        {
            return -1;
        }
        return _states[detent];
    }
    internal Detent GetDetentForState(int state)
    {
        return _states.FirstOrDefault(kv => kv.Value == state).Key;
    }

    public void Dismiss(bool animated)
    {

        if (animated)
        {
            _backdrop?.AnimateOut();
            Behavior.Hideable = true;
            Behavior.State = BottomSheetBehavior.StateHidden;
        }
        else
        {
            Dispose();
        }
    }

    void Dispose()
    {
        _layout.LayoutChange -= OnLayoutChange;
        var navigationRootManager = _windowMauiContext.Services.GetRequiredService<NavigationRootManager>();
        if (navigationRootManager.RootView is CoordinatorLayout coordinatorLayout && _coordinatorLayout == coordinatorLayout)
        {
            _frame.RemoveFromParent();
        }
        else
        {
            _coordinatorLayout.RemoveFromParent();
        }
        _frame = null;
        _coordinatorLayout = null;
        _backdrop.RemoveFromParent();
        _backdrop = null;
    }

    public void Layout()
    {
        // TODO: verify that, maybe handle statusbar and navigationbar
        var maxSheetHeight = _sheet.Window.Height;
        BottomSheetManager.LayoutDetents(_behavior, _frame, _heights, maxSheetHeight);
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

        var container =
            (FrameLayout)AView.Inflate(_windowMauiContext.Context, Resource.Layout.the49_maui_bottom_sheet_design, null);

        var coordinator = (CoordinatorLayout)container.FindViewById(Resource.Id.coordinator);
        _frame = (FrameLayout)container.FindViewById(Resource.Id.design_bottom_sheet);

        _behavior = BottomSheetBehavior.From(_frame);

        var navigationRootManager = _windowMauiContext.Services.GetRequiredService<NavigationRootManager>();

        if (navigationRootManager.RootView is ContainerView cv && cv.MainView is DrawerLayout drawerLayout)
        {
            _coordinatorLayout = new CoordinatorLayout(_windowMauiContext.Context);

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

        if (_sheet.HasBackdrop)
        {
            _backdrop = new BottomSheetBackdrop(_windowMauiContext.Context);
            _backdrop.Click += BackdropClicked;

            _coordinatorLayout.AddView(_backdrop, new CoordinatorLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        }

        _coordinatorLayout.AddView(container, new CoordinatorLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

        ViewCompat.SetOnApplyWindowInsetsListener(_frame, new OnApplyWindowInsetsListener(this));
    }

    private void BackdropClicked(object sender, EventArgs e)
    {
        Dismiss(true);
    }

    public void Show(bool animated)
    {
        _isDuringShowingAnimation = true;
        SetupCoordinatorLayout();

        _layout = BottomSheetManager.CreateLayout(_sheet, _windowMauiContext);

        _layout.LayoutChange += OnLayoutChange;

        _frame.AddView(_layout);

        var callback = new BottomSheetCallback(_sheet);
        callback.StateChanged += Callback_StateChanged;
        Behavior.AddBottomSheetCallback(callback);

        if (animated)
        {
            _backdrop?.AnimateIn();
            _behavior.State = BottomSheetBehavior.StateHidden;
        }

        _sheet.Dispatcher.Dispatch(() =>
        {
            UpdateBackground();
            Layout();
            CalculateHeights(_sheet, _sheet.Window.Height);
            CalculateStates();

            var state = GetStateForDetent(_sheet.SelectedDetent);

            var defaultDetent = _sheet.GetDefaultDetent();
            if (state is -1)
            {
                state = Behavior.SkipCollapsed ? BottomSheetBehavior.StateExpanded : BottomSheetBehavior.StateCollapsed;
            }

            Behavior.State = state;

            _sheet.NotifyShowing();
        });
    }

    void OnLayoutChange(object sender, AView.LayoutChangeEventArgs e)
    {
        CalculateHeights(_sheet, _sheet.Window.Height);
        CalculateStates();
        Layout();
    }

    void Callback_StateChanged(object sender, EventArgs e)
    {
        if (_isDuringShowingAnimation && (
            Behavior.State == BottomSheetBehavior.StateCollapsed
            || Behavior.State == BottomSheetBehavior.StateHalfExpanded
            || Behavior.State == BottomSheetBehavior.StateExpanded
            ))
        {
            _isDuringShowingAnimation = false;
            Behavior.Hideable = _sheet.IsCancelable;
            _sheet.NotifyShown();
        }
        if (Behavior.State == BottomSheetBehavior.StateHidden)
        {
            _sheet.NotifyDismissed();
            Dispose();
        }
        ((BottomSheetHandler)_sheet.Handler).UpdateSelectedDetent(_sheet);
    }

    internal void UpdateSelectedDetent()
    {
        var detent = GetDetentForState(Behavior.State);
        if (detent is not null)
        {
            _sheet.SelectedDetent = detent;
        }
    }

    internal void UpdateStateFromDetent()
    {
        if (_sheet.SelectedDetent is null || Behavior is null)
        {
            return;
        }
        Behavior.State = GetStateForDetent(_sheet.SelectedDetent);
    }
}
