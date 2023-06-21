using Android.Views;
using Microsoft.Maui.Platform;
using Google.Android.Material.BottomSheet;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.DrawerLayout.Widget;
using Android.Content.Res;
using AView = Android.Views.View;
using Exception = System.Exception;
using AndroidX.Core.View;
using Android.App;
using AndroidX.Core.Graphics;
using Insets = AndroidX.Core.Graphics.Insets;

namespace The49.Maui.BottomSheet;

public class BottomSheetController
{
    class EdgeToEdgeListener : Java.Lang.Object, IOnApplyWindowInsetsListener
    {
        BottomSheetController _controller;
        public EdgeToEdgeListener(BottomSheetController controller)
        {
            _controller = controller;
        }
        public WindowInsetsCompat OnApplyWindowInsets(AView v, WindowInsetsCompat insets)
        {
            var i = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());
            _controller._insets = i;
            return ViewCompat.OnApplyWindowInsets(v, insets);
        }
    }
    internal IDictionary<Detent, int> _states;
    internal IDictionary<Detent, double> _heights;

    Insets _insets;

    bool _isDuringShowingAnimation = false;

    BottomSheetBehavior _behavior;
    public BottomSheetBehavior Behavior => _behavior;

    ViewGroup _frame;
    ViewGroup _layout;
    BottomSheetBackdrop _backdrop;
    FrameLayout _container;

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
            _sheet.NotifyDismissed();
        }
    }

    void Dispose()
    {
        _layout.LayoutChange -= OnLayoutChange;

        _container.RemoveFromParent();
        _backdrop?.RemoveFromParent();

        _frame = null;
        _container = null;
        _backdrop = null;

        var window = ((Activity)_windowMauiContext.Context).Window;
        WindowCompat.SetDecorFitsSystemWindows(window, false);
    }

    public void Layout()
    {
        BottomSheetManager.LayoutDetents(_behavior, _frame, _heights, GetAvailableHeight());
    }

    public void UpdateBackground()
    {
        Paint paint = _sheet.BackgroundBrush;
        if (_frame != null && paint != null)
        {
            _frame.BackgroundTintList = ColorStateList.ValueOf(paint.ToColor().ToPlatform());
        }
    }

    ViewGroup FindNavigationRootView()
    {
        var navigationRootManager = _windowMauiContext.Services.GetRequiredService<NavigationRootManager>();

        if (navigationRootManager.RootView is ContainerView cv && cv.MainView is DrawerLayout drawerLayout)
        {
            return drawerLayout;
        }
        if (navigationRootManager.RootView is CoordinatorLayout coordinatorLayout)
        {
            return coordinatorLayout;
        }

        throw new Exception("Unrecognized RootView");

    }

    void SetupCoordinatorLayout()
    {
        _container =
            (FrameLayout)AView.Inflate(_windowMauiContext.Context, Resource.Layout.the49_maui_bottom_sheet_design, null);

        _container.ViewAttachedToWindow += ContainerAttachedToWindow;
        _container.ViewDetachedFromWindow += ContainerDetachedFromWindow;

        _frame = (FrameLayout)_container.FindViewById(Resource.Id.design_bottom_sheet);

        _frame.OutlineProvider = ViewOutlineProvider.Background;
        _frame.ClipToOutline = true;

        ViewCompat.SetOnApplyWindowInsetsListener(_container, new EdgeToEdgeListener(this));

        _behavior = BottomSheetBehavior.From(_frame);

        var rootView = FindNavigationRootView();

        if (_sheet.HasBackdrop)
        {
            _backdrop = new BottomSheetBackdrop(_windowMauiContext.Context);
            _backdrop.Click += BackdropClicked;

            rootView.AddView(_backdrop, new CoordinatorLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        }

        rootView.AddView(_container, new CoordinatorLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
    }

    void ContainerDetachedFromWindow(object sender, AView.ViewDetachedFromWindowEventArgs e)
    {
        var window = ((Activity)_windowMauiContext.Context).Window;
        WindowCompat.SetDecorFitsSystemWindows(window, false);
    }

    void ContainerAttachedToWindow(object sender, AView.ViewAttachedToWindowEventArgs e) { }

    private void BackdropClicked(object sender, EventArgs e)
    {
        Dismiss(true);
    }

    public double GetAvailableHeight()
    {
        var density = DeviceDisplay.MainDisplayInfo.Density;

        return (_container.Height - (_insets?.Top ?? 0) - (_insets?.Bottom ?? 0)) / density;
    }

    public void Show(bool animated)
    {
        _isDuringShowingAnimation = true;
        SetupCoordinatorLayout();

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
            _layout = BottomSheetManager.CreateLayout(_sheet, _windowMauiContext);

            _layout.LayoutChange += OnLayoutChange;

            _frame.AddView(_layout);
            UpdateBackground();
            var h = GetAvailableHeight();
            CalculateHeights(_sheet, GetAvailableHeight());
            CalculateStates();
            Layout();

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
        CalculateHeights(_sheet, GetAvailableHeight());
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
        if (_sheet.SelectedDetent is null || Behavior is null || _states is null)
        {
            return;
        }
        Behavior.State = GetStateForDetent(_sheet.SelectedDetent);
    }
}
