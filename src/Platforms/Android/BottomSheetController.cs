using Android.Views;
using Microsoft.Maui.Platform;
using Google.Android.Material.BottomSheet;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Android.Content.Res;
using AView = Android.Views.View;
using AWindow = Android.Views.Window;
using AndroidX.Core.View;
using Android.App;
using Insets = AndroidX.Core.Graphics.Insets;
using AndroidX.AppCompat.App;
using Google.Android.Material.Internal;

namespace The49.Maui.BottomSheet;

public class BottomSheetController
{
    class EdgeToEdgeCallback : BottomSheetBehavior.BottomSheetCallback
    {

        bool? lightBottomSheet;
        WindowInsetsCompat _insetsCompat;

        AWindow window;
        bool lightStatusBar;

        public EdgeToEdgeCallback(
            AView bottomSheet, WindowInsetsCompat insetsCompat)
        {
            this._insetsCompat = insetsCompat;

            // Try to find the background color to automatically change the status bar icons so they will
            // still be visible when the bottomsheet slides underneath the status bar.
            ColorStateList backgroundTint;

            backgroundTint = ViewCompat.GetBackgroundTintList(bottomSheet);

            //if (backgroundTint != null)
            //{
            //    // First check for a tint
            //    lightBottomSheet = isColorLight(backgroundTint.getDefaultColor());
            //}
            //else if (bottomSheet.getBackground() instanceof ColorDrawable) {
            //    // Then check for the background color
            //    lightBottomSheet = isColorLight(((ColorDrawable)bottomSheet.getBackground()).getColor());
            //} else
            //{
            //    // Otherwise don't change the status bar color
            //    lightBottomSheet = null;
            //}
        }

        public override void OnStateChanged(AView bottomSheet, int p1)
        {
            SetPaddingForPosition(bottomSheet);
        }

        public override void OnSlide(AView bottomSheet, float newState)
        {
            SetPaddingForPosition(bottomSheet);
        }

        public void SetWindow(AWindow window)
        {
            if (this.window == window)
            {
                return;
            }
            this.window = window;
            if (window != null)
            {
                WindowInsetsControllerCompat insetsController =
                    WindowCompat.GetInsetsController(window, window.DecorView);
                lightStatusBar = insetsController.AppearanceLightStatusBars;
            }
        }

        void SetPaddingForPosition(AView bottomSheet)
        {
            if (bottomSheet.Top < _insetsCompat.SystemWindowInsetTop)
            {
                // If the bottomsheet is light, we should set light status bar so the icons are visible
                // since the bottomsheet is now under the status bar.
                if (window != null)
                {
                    EdgeToEdgeUtils.SetLightStatusBar(
                        window, !lightBottomSheet.HasValue ? lightStatusBar : lightBottomSheet.Value);
                }
                // Smooth transition into status bar when drawing edge to edge.
                bottomSheet.SetPadding(
                    bottomSheet.PaddingLeft,
                    _insetsCompat.SystemWindowInsetTop - bottomSheet.Top,
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
    class EdgeToEdgeListener : Java.Lang.Object, IOnApplyWindowInsetsListener
    {
        BottomSheetController _controller;
        EdgeToEdgeCallback _edgeToEdgeCallback;
        public EdgeToEdgeListener(BottomSheetController controller)
        {
            _controller = controller;
        }
        public WindowInsetsCompat OnApplyWindowInsets(AView v, WindowInsetsCompat insets)
        {
            var i = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());
            _controller._insets = i;

            if (_edgeToEdgeCallback is not null)
            {
                _controller.Behavior.RemoveBottomSheetCallback(_edgeToEdgeCallback);
            }

            if (insets != null)
            {
                _edgeToEdgeCallback = new EdgeToEdgeCallback(_controller._frame, insets);
                _edgeToEdgeCallback.SetWindow(((AppCompatActivity)_controller._windowMauiContext.Context).Window);
                _controller.Behavior.AddBottomSheetCallback(_edgeToEdgeCallback);
            }

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
    FrameLayout _windowContainer;
    private BottomSheetDragHandleView _handle;

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
        //WindowCompat.SetDecorFitsSystemWindows(window, false);
    }

    public void Layout()
    {
        LayoutDetents(_behavior, _frame, _heights, GetAvailableHeight());
    }

    internal void UpdateBackground()
    {
        Paint paint = _sheet.BackgroundBrush;
        if (_frame != null && paint != null)
        {
            _frame.BackgroundTintList = ColorStateList.ValueOf(paint.ToColor().ToPlatform());
        }
    }

    public void UpdateHandleColor()
    {
        if (_handle is null)
        {
            return;
        }
        if (_sheet.HandleColor is not null)
        {
            _handle.SetColorFilter(_sheet.HandleColor.ToPlatform());
        }
    }

    void EnsureWindowContainer()
    {
        if (_windowContainer is null)
        {
            var windowPv = _sheet.Window.Handler.PlatformView as AppCompatActivity;
            var window = windowPv.Window;
            _windowContainer = new FrameLayout(_windowMauiContext.Context);
            window.AddContentView(_windowContainer, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        }
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

        EnsureWindowContainer();

        _windowContainer.RemoveAllViews();

        if (_sheet.HasBackdrop)
        {
            _backdrop = new BottomSheetBackdrop(_windowMauiContext.Context);
            _backdrop.Click += BackdropClicked;

            _windowContainer.AddView(_backdrop, new CoordinatorLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        }

        _windowContainer.AddView(_container, new CoordinatorLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
    }

    void ContainerDetachedFromWindow(object sender, AView.ViewDetachedFromWindowEventArgs e)
    {

    }

    void ContainerAttachedToWindow(object sender, AView.ViewAttachedToWindowEventArgs e) { }

    private void BackdropClicked(object sender, EventArgs e)
    {
        Dismiss(true);
    }

    public double GetAvailableHeight()
    {
        var density = DeviceDisplay.MainDisplayInfo.Density;

        return (_container.Height) / density;
    }

    ViewGroup CreateLayout()
    {
        // The Android view for the page could already have a ContainerView as a parent if it was shown as a bottom sheet before
        if (((AView)_sheet.Handler?.PlatformView)?.Parent is ContainerView cv)
        {
            cv.RemoveAllViews();
        }
        var containerView = _sheet.ToContainerView(_windowMauiContext);

        var r = _sheet.Measure(_sheet.Window.Width, GetAvailableHeight());

        containerView.LayoutParameters = new(ViewGroup.LayoutParams.MatchParent, (int)Math.Round(r.Request.Height * DeviceDisplay.MainDisplayInfo.Density));
        var layout = new FrameLayout(_windowMauiContext.Context);
        if (_sheet.HasHandle)
        {
            _handle = new BottomSheetDragHandleView(_windowMauiContext.Context);
            UpdateHandleColor();
            layout.AddView(_handle, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
        }
        layout.AddView(containerView);

        return layout;
    }

    internal static void LayoutDetents(BottomSheetBehavior behavior, ViewGroup container, IDictionary<Detent, double> heights, double maxSheetHeight)
    {
        // Android supports the following detents:
        // - expanded (top of screen - offset)
        // - half expanded (using ratio of expanded - peekHeight)
        // - collapsed (using peekHeight)

        var sortedHeights = heights
            .OrderByDescending(i => i.Value)
            .ToList();


        if (sortedHeights.Count == 1)
        {
            behavior.FitToContents = true;
            behavior.SkipCollapsed = true;
            var top = sortedHeights[0].Value;
            container.LayoutParameters.Height = (int)(top * DeviceDisplay.MainDisplayInfo.Density);
        }
        else if (sortedHeights.Count == 2)
        {
            behavior.FitToContents = true;
            behavior.SkipCollapsed = false;
            var top = sortedHeights[0].Value;
            container.LayoutParameters.Height = (int)(top * DeviceDisplay.MainDisplayInfo.Density);
            var bottom = sortedHeights[1].Value;

            behavior.PeekHeight = (int)(bottom * DeviceDisplay.MainDisplayInfo.Density);
        }
        else if (sortedHeights.Count == 3)
        {
            behavior.FitToContents = false;
            behavior.SkipCollapsed = false;
            var top = sortedHeights[0].Value;
            var midway = sortedHeights[1].Value;
            var bottom = sortedHeights[2].Value;

            container.LayoutParameters.Height = (int)(top * DeviceDisplay.MainDisplayInfo.Density);

            // Set the top detent by offsetting the requested height from the maxHeight
            var topOffset = (maxSheetHeight - top) * DeviceDisplay.MainDisplayInfo.Density;
            behavior.ExpandedOffset = (int)topOffset;

            // Set the midway detent by calculating the ratio using the top detent info
            var ratio = midway / top;
            behavior.HalfExpandedRatio = (float)ratio;

            // Set the bottom detent using the peekHeight
            behavior.PeekHeight = (int)(bottom * DeviceDisplay.MainDisplayInfo.Density);
        }

        container.RequestLayout();
    }

    AWindow Window => ((AppCompatActivity)_windowMauiContext.Context).Window;

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
            _layout = CreateLayout();

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
