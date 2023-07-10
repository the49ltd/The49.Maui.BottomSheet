using Android.Views;
using Microsoft.Maui.Platform;
using Google.Android.Material.BottomSheet;
using Android.Widget;
using Android.Content.Res;
using AView = Android.Views.View;
using AWindow = Android.Views.Window;
using AndroidX.Core.View;
using AndroidX.AppCompat.App;
using Google.Android.Material.Internal;
using Google.Android.Material.Color;
using Android.Graphics.Drawables;
using Android.Content;

namespace The49.Maui.BottomSheet;

public class BottomSheetController
{
    class EdgeToEdgeCallback : BottomSheetBehavior.BottomSheetCallback
    {
        private BottomSheetController _controller;
        WindowInsetsCompat _insetsCompat;

        AWindow _window;
        bool _isStatusBarLight;

        public EdgeToEdgeCallback(BottomSheetController controller, WindowInsetsCompat insetsCompat)
        {
            _controller = controller;
            _insetsCompat = insetsCompat;
            SetPaddingForPosition(_controller._frame);
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
            if (_window == window)
            {
                return;
            }
            _window = window;
            if (window != null)
            {
                WindowInsetsControllerCompat insetsController = WindowCompat.GetInsetsController(window, window.DecorView);
                _isStatusBarLight = insetsController.AppearanceLightStatusBars;
            }
        }

        void SetPaddingForPosition(AView bottomSheet)
        {
            var keyboardHeight = _insetsCompat.GetInsets(WindowInsetsCompat.Type.Ime()).Bottom;
            if (bottomSheet.Top < _insetsCompat.StableInsetTop)
            {
                // If the bottomsheet is light, we should set light status bar so the icons are visible
                // since the bottomsheet is now under the status bar.
                if (_window != null)
                {
                    EdgeToEdgeUtils.SetLightStatusBar(
                        _window, !_controller._isBackgroundLight.HasValue ? _isStatusBarLight : _controller._isBackgroundLight.Value);
                }
                // Smooth transition into status bar when drawing edge to edge.
                bottomSheet.SetPadding(
                    bottomSheet.PaddingLeft,
                    _insetsCompat.StableInsetTop - bottomSheet.Top,
                    bottomSheet.PaddingRight,
                    keyboardHeight);
            }
            else if (bottomSheet.Top != 0)
            {
                // Reset the status bar icons to the original color because the bottomsheet is not under the
                // status bar.
                if (_window != null)
                {
                    EdgeToEdgeUtils.SetLightStatusBar(_window, _isStatusBarLight);
                }
                bottomSheet.SetPadding(
                    bottomSheet.PaddingLeft,
                    0,
                    bottomSheet.PaddingRight,
                    keyboardHeight);
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
            if (_edgeToEdgeCallback is not null)
            {
                _controller.Behavior.RemoveBottomSheetCallback(_edgeToEdgeCallback);
            }

            if (insets != null)
            {
                _edgeToEdgeCallback = new EdgeToEdgeCallback(_controller, insets);
                _edgeToEdgeCallback.SetWindow(((AppCompatActivity)_controller._mauiContext.Context).Window);
                _controller.Behavior.AddBottomSheetCallback(_edgeToEdgeCallback);
                _controller.CalculateHeights(_controller.GetAvailableHeight());
                _controller.ResizeVirtualView();
                _controller.Layout();
            }


            return ViewCompat.OnApplyWindowInsets(v, insets);
        }
    }

    class BottomSheetInsetsAnimationCallback : WindowInsetsAnimationCompat.Callback
    {
        readonly BottomSheetController _controller;
        int _startHeight;
        int _endHeight;

        WindowInsetsCompat GetInsets()
        {
            if (!OperatingSystem.IsAndroidVersionAtLeast(23))
            {
                return new WindowInsetsCompat.Builder().Build();
            }

            return WindowInsetsCompat.ToWindowInsetsCompat(_controller._windowContainer.RootWindowInsets);
        }

        public BottomSheetInsetsAnimationCallback(BottomSheetController controller) : base(DispatchModeStop)
        {
            _controller = controller;
        }

        public override WindowInsetsAnimationCompat.BoundsCompat OnStart(WindowInsetsAnimationCompat animation, WindowInsetsAnimationCompat.BoundsCompat bounds)
        {
            var insets = GetInsets();

            _endHeight = insets.GetInsets(WindowInsetsCompat.Type.Ime()).Bottom;
            _controller._frame.TranslationY = _endHeight - _startHeight;
            return bounds;
        }

        public override void OnPrepare(WindowInsetsAnimationCompat animation)
        {
            var insets = GetInsets();
            _startHeight = insets.GetInsets(WindowInsetsCompat.Type.Ime()).Bottom;
            base.OnPrepare(animation);
        }

        public override WindowInsetsCompat OnProgress(WindowInsetsCompat insets, IList<WindowInsetsAnimationCompat> runningAnimations)
        {
            WindowInsetsAnimationCompat imeAnimation = null;
            foreach (var animation in runningAnimations)
            {
                if ((animation.TypeMask & WindowInsetsCompat.Type.Ime()) != 0)
                {
                    imeAnimation = animation;
                    break;
                }
            }
            if (imeAnimation != null)
            {
                _controller._frame.TranslationY = (_endHeight - _startHeight) * (1 - imeAnimation.InterpolatedFraction);
            }
            return insets;
        }
    }

    static StayOnFrontView _stayOnFront;

    internal IDictionary<Detent, int> _states;
    internal IDictionary<Detent, double> _heights;
    bool _isDuringShowingAnimation = false;
    BottomSheetBehavior _behavior;
    ViewGroup _frame;
    BottomSheetContainer _windowContainer;
    BottomSheetDragHandleView _handle;
    bool? _isBackgroundLight;

    public BottomSheetBehavior Behavior => _behavior;

    IMauiContext _mauiContext { get; }
    BottomSheet _sheet { get; }

    public bool UseNavigationBarArea { get; set; } = false;

    int BottomInset => UseNavigationBarArea ? 0 : _windowContainer.RootWindowInsets.StableInsetBottom;

    public BottomSheetController(IMauiContext windowMauiContext, BottomSheet sheet)
    {
        _mauiContext = windowMauiContext;
        _sheet = sheet;
    }

    internal void CalculateHeights(double maxSheetHeight)
    {
        var detents = _sheet.GetEnabledDetents().ToList();

        _heights = new Dictionary<Detent, double>();

        foreach (var detent in detents)
        {
            _heights.Add(detent, detent.GetHeight(_sheet, maxSheetHeight));
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
            _windowContainer?.Backdrop.AnimateOut();
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
        _frame.LayoutChange -= OnLayoutChange;
        _windowContainer.RemoveFromParent();
    }

    public void Layout()
    {
        LayoutDetents(_heights, GetAvailableHeight());
    }

    internal void UpdateBackground()
    {
        Paint paint = _sheet.BackgroundBrush;
        if (_frame != null && paint != null)
        {
            _frame.BackgroundTintList = ColorStateList.ValueOf(paint.ToColor().ToPlatform());
        }
        // Try to find the background color to automatically change the status bar icons so they will
        // still be visible when the bottomsheet slides underneath the status bar.
        ColorStateList backgroundTint = ViewCompat.GetBackgroundTintList(_frame);

        if (backgroundTint != null)
        {
            // First check for a tint
            _isBackgroundLight = MaterialColors.IsColorLight(backgroundTint.DefaultColor);
        }
        else if (_frame.Background is ColorDrawable)
        {
            // Then check for the background color
            _isBackgroundLight = MaterialColors.IsColorLight(((ColorDrawable)_frame.Background).Color);
        }
        else
        {
            // Otherwise don't change the status bar color
            _isBackgroundLight = null;
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

    static void EnsureStayOnFrontView(Context context)
    {
        if (_stayOnFront is null)
        {
            _stayOnFront = new StayOnFrontView(context);
            var window = ((AppCompatActivity)context).Window;
            var parentView = window?.DecorView as ViewGroup;
            parentView.AddView(_stayOnFront);
        }
    }

    void EnsureWindowContainer()
    {
        EnsureStayOnFrontView(_mauiContext.Context);
        if (_windowContainer is null)
        {
            var container = (FrameLayout)AView.Inflate(_mauiContext.Context, Resource.Layout.the49_maui_bottom_sheet_design, null);

            container.ViewAttachedToWindow += ContainerAttachedToWindow;
            container.ViewDetachedFromWindow += ContainerDetachedFromWindow;

            _windowContainer = new BottomSheetContainer(_mauiContext.Context, container);
            _windowContainer.Backdrop.Click += BackdropClicked;

            _frame = (FrameLayout)container.FindViewById(Resource.Id.design_bottom_sheet);

            _frame.OutlineProvider = ViewOutlineProvider.Background;
            _frame.ClipToOutline = true;

            ViewCompat.SetOnApplyWindowInsetsListener(_windowContainer, new EdgeToEdgeListener(this));
            ViewCompat.SetWindowInsetsAnimationCallback(_frame, new BottomSheetInsetsAnimationCallback(this));

            _behavior = BottomSheetBehavior.From(_frame);

            var callback = new BottomSheetCallback(_sheet);
            callback.StateChanged += Callback_StateChanged;
            _behavior.AddBottomSheetCallback(callback);
        }
    }

    void ContainerDetachedFromWindow(object sender, AView.ViewDetachedFromWindowEventArgs e)
    {

    }

    void ContainerAttachedToWindow(object sender, AView.ViewAttachedToWindowEventArgs e)
    {

    }

    void BackdropClicked(object sender, EventArgs e)
    {
        if (_sheet.IsCancelable)
        {
            Dismiss(true);
        }
    }

    public double GetAvailableHeight()
    {
        var density = DeviceDisplay.MainDisplayInfo.Density;

        var insets = _windowContainer.RootWindowInsets;

        var keyboardHeight = insets.GetInsets(WindowInsetsCompat.Type.Ime()).Bottom;

        return (_windowContainer.Height - insets.StableInsetTop - BottomInset - keyboardHeight) / density;
    }

    internal void LayoutDetents(IDictionary<Detent, double> heights, double maxSheetHeight)
    {
        // Android supports the following detents:
        // - expanded (top of screen - offset)
        // - half expanded (using ratio of expanded - peekHeight)
        // - collapsed (using peekHeight)

        var sortedHeights = heights
            .OrderByDescending(i => i.Value)
            .ToList();
        var density = DeviceDisplay.MainDisplayInfo.Density;
        var insets = _windowContainer.RootWindowInsets;
        var topInset = insets.StableInsetTop;
        var keyboardHeight = insets.GetInsets(WindowInsetsCompat.Type.Ime()).Bottom;

        var top = sortedHeights[0].Value;

        // Configure the sheet to handle up to 3 detents

        if (sortedHeights.Count == 1)
        { // Only way to have one detent on Android is to use fitToContent. Use that
            _behavior.FitToContents = true;
            _behavior.SkipCollapsed = true;
        }
        else if (sortedHeights.Count == 2)
        { // We can handle a second detent by adding a collapsed state. Use peek height
            _behavior.FitToContents = true;
            _behavior.SkipCollapsed = false;

            var bottom = sortedHeights[1].Value;

            _behavior.PeekHeight = (int)(bottom * density) + BottomInset + keyboardHeight;
        }
        else if (sortedHeights.Count == 3)
        { // 3 detents can be done using the peek height AND disabling fitToContent
          // Doing so uses a property called halfExpandedRatio, giving us
          // Expanded: Use ExpandedOffset to offset from the top
          // HalfExpanded: Use HalfExpandedRatio
          // Collapsed: Use PeekHeight

            _behavior.FitToContents = false;
            _behavior.SkipCollapsed = false;

            var midway = sortedHeights[1].Value;
            var bottom = sortedHeights[2].Value;

            // Set the top detent by offsetting the requested height from the maxHeight
            var topOffset = (maxSheetHeight - top) * density;
            _behavior.ExpandedOffset = Math.Max(0, (int)topOffset);

            // Set the midway detent by calculating the ratio using the top detent info
            var ratio = ((midway * density) + keyboardHeight + BottomInset) / _frame.LayoutParameters.Height;
            _behavior.HalfExpandedRatio = (float)ratio;

            // Set the bottom detent using the peekHeight
            _behavior.PeekHeight = (int)(bottom * density) + BottomInset + keyboardHeight;
        }
    }

    double CalculateTallestDetent(double heightConstraint)
    {
        if (_heights is null)
        {
            CalculateHeights(heightConstraint);
        }
        return _heights.Values.Max();
    }

    void ResizeVirtualView()
    {
        var pv = (ContentViewGroup)_sheet.Handler?.PlatformView;
        var maxHeight = GetAvailableHeight();
        var height = CalculateTallestDetent(maxHeight);

        double density = DeviceDisplay.MainDisplayInfo.Density;

        var platformHeight = (int)Math.Round(height * density);

        pv.LayoutParameters = new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            platformHeight
        );

        int keyboardHeight = 0;
        int topInset = 0;

        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            var insets = _windowContainer.RootWindowInsets;
            var compat = WindowInsetsCompat.ToWindowInsetsCompat(insets);
            keyboardHeight = compat.GetInsets(WindowInsetsCompat.Type.Ime()).Bottom;
            topInset = compat.StableInsetTop;
        }

        var layoutParams = _frame.LayoutParameters;

        layoutParams.Height = platformHeight + BottomInset + keyboardHeight;

        if (height == maxHeight)
        {
            layoutParams.Height += topInset;
        }
        _sheet.Arrange(new Rect(0, 0, _frame.Width / density, height));
        _frame.RequestLayout();
    }

    public void Show(bool animated)
    {
        _isDuringShowingAnimation = true;

        EnsureWindowContainer();

        _stayOnFront.AddView(_windowContainer);

        _frame.RemoveAllViews();

        // The Android view for the page could already have a ContainerView as a parent if it was shown as a bottom sheet before
        ((ContentViewGroup)_sheet.Handler?.PlatformView)?.RemoveFromParent();
        var containerView = _sheet.ToPlatform(_mauiContext);

        var c = new FrameLayout(_mauiContext.Context);

        if (_sheet.HasHandle)
        {
            _handle = new BottomSheetDragHandleView(_mauiContext.Context);
            c.AddView(_handle, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
        }

        c.AddView(containerView);

        _frame.AddView(c);

        UpdateBackground();
        UpdateHasBackdrop();
        UpdateHandleColor();

        if (animated)
        {
            _windowContainer?.Backdrop.AnimateIn();
            _behavior.State = BottomSheetBehavior.StateHidden;
        }

        _sheet.Dispatcher.Dispatch(() =>
        {
            ResizeVirtualView();

            CalculateHeights(GetAvailableHeight());
            CalculateStates();
            Layout();

            var state = GetStateForDetent(_sheet.SelectedDetent);

            var defaultDetent = _sheet.GetDefaultDetent();
            if (state is -1)
            {
                state = Behavior.SkipCollapsed ? BottomSheetBehavior.StateExpanded : BottomSheetBehavior.StateCollapsed;
            }

            Behavior.State = state;

            c.LayoutChange += OnLayoutChange;

            _sheet.NotifyShowing();
        });
    }

    void OnLayoutChange(object sender, AView.LayoutChangeEventArgs e)
    {
        CalculateHeights(GetAvailableHeight());
        CalculateStates();
        ResizeVirtualView();
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

    internal void UpdateHasBackdrop()
    {
        _windowContainer?.SetBackdropVisibility(_sheet.HasBackdrop);
    }
}
