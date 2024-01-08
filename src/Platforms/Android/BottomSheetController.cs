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
using Insets = AndroidX.Core.Graphics.Insets;

namespace The49.Maui.BottomSheet;

public class BottomSheetController
{
    class EdgeToEdgeCallback : BottomSheetBehavior.BottomSheetCallback
    {
        BottomSheetController controller;
        WindowInsetsCompat insetsCompat;

        AWindow window;
        bool isStatusBarLight;

        public EdgeToEdgeCallback(BottomSheetController controller, WindowInsetsCompat insetsCompat)
        {
            this.controller = controller;
            this.insetsCompat = insetsCompat;
			SetPaddingForPosition(this.controller.frame);
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
                WindowInsetsControllerCompat insetsController = WindowCompat.GetInsetsController(window, window.DecorView);
                isStatusBarLight = insetsController.AppearanceLightStatusBars;
            }
        }

        int TopInset
        {
            get
            {
                if (OperatingSystem.IsAndroidVersionAtLeast(30))
                {
                    return insetsCompat.GetInsetsIgnoringVisibility(Android.Views.WindowInsets.Type.SystemBars()).Top;
                }
#pragma warning disable CS0618
                return insetsCompat.StableInsetTop;
#pragma warning restore CS0618
            }
        }

        void SetPaddingForPosition(AView bottomSheet)
        {
            var keyboardHeight = insetsCompat.GetInsets(WindowInsetsCompat.Type.Ime()).Bottom;
            if (bottomSheet.Top < TopInset)
            {
                // If the bottomsheet is light, we should set light status bar so the icons are visible
                // since the bottomsheet is now under the status bar.
                if (window != null)
                {
                    EdgeToEdgeUtils.SetLightStatusBar(
                        window, !controller.isBackgroundLight.HasValue ? isStatusBarLight : controller.isBackgroundLight.Value);
                }
                // Smooth transition into status bar when drawing edge to edge.
                bottomSheet.SetPadding(
                    bottomSheet.PaddingLeft,
                    TopInset - bottomSheet.Top,
                    bottomSheet.PaddingRight,
                    keyboardHeight);
            }
            else if (bottomSheet.Top != 0)
            {
                // Reset the status bar icons to the original color because the bottomsheet is not under the
                // status bar.
                if (window != null)
                {
                    EdgeToEdgeUtils.SetLightStatusBar(window, isStatusBarLight);
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
        BottomSheetController controller;
        EdgeToEdgeCallback edgeToEdgeCallback;

        public EdgeToEdgeListener(BottomSheetController controller)
        {
            this.controller = controller;
        }
        public WindowInsetsCompat OnApplyWindowInsets(AView v, WindowInsetsCompat insets)
        {
            if (edgeToEdgeCallback is not null)
            {
                controller.Behavior.RemoveBottomSheetCallback(edgeToEdgeCallback);
            }

            if (insets != null)
            {
                edgeToEdgeCallback = new EdgeToEdgeCallback(controller, insets);
                edgeToEdgeCallback.SetWindow(((AppCompatActivity)controller._mauiContext.Context).Window);
                controller.Behavior.AddBottomSheetCallback(edgeToEdgeCallback);
                controller.CalculateHeights(controller.GetAvailableHeight());
                controller.ResizeVirtualView();
                controller.Layout();
            }


            return ViewCompat.OnApplyWindowInsets(v, insets);
        }
    }

    class BottomSheetInsetsAnimationCallback : WindowInsetsAnimationCompat.Callback
    {
        readonly BottomSheetController controller;
        int startHeight;
        int endHeight;

        public BottomSheetInsetsAnimationCallback(BottomSheetController controller) : base(DispatchModeStop)
        {
            this.controller = controller;
        }

        public override WindowInsetsAnimationCompat.BoundsCompat OnStart(WindowInsetsAnimationCompat animation, WindowInsetsAnimationCompat.BoundsCompat bounds)
        {
            endHeight = controller.WindowInsets.GetInsets(WindowInsetsCompat.Type.Ime()).Bottom;
            controller.frame.TranslationY = endHeight - startHeight;
            return bounds;
        }

        public override void OnPrepare(WindowInsetsAnimationCompat animation)
        {
            startHeight = controller.WindowInsets.GetInsets(WindowInsetsCompat.Type.Ime()).Bottom;
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
                controller.frame.TranslationY = (endHeight - startHeight) * (1 - imeAnimation.InterpolatedFraction);
            }
            return insets;
        }
    }

    static StayOnFrontView stayOnFront;

    internal IDictionary<Detent, int> States;
    internal IDictionary<Detent, double> Heights;
    bool isDuringShowingAnimation = false;
    BottomSheetBehavior behavior;
    ViewGroup frame;
    BottomSheetContainer windowContainer;
    BottomSheetDragHandleView handle;
    bool? isBackgroundLight;

    public ViewGroup Frame => frame;

    public BottomSheetBehavior Behavior => behavior;

    IMauiContext _mauiContext { get; }
    BottomSheet _sheet { get; }

    public bool UseNavigationBarArea { get; set; } = false;

    int BottomInset => UseNavigationBarArea ? 0 : Insets.Bottom;

    public BottomSheetController(IMauiContext windowMauiContext, BottomSheet sheet)
    {
        _mauiContext = windowMauiContext;
        _sheet = sheet;
    }

    internal void CalculateHeights(double maxSheetHeight)
    {
        var detents = _sheet.GetEnabledDetents().ToList();

        Heights = new Dictionary<Detent, double>();

        foreach (var detent in detents)
        {
            Heights.Add(detent, detent.GetHeight(_sheet, maxSheetHeight));
        }
    }

    internal void CalculateStates()
    {
        var heights = Heights.OrderByDescending(kv => kv.Value).ToList();

        States = new Dictionary<Detent, int>();

        if (heights.Count == 1)
        {
            States.Add(heights[0].Key, BottomSheetBehavior.StateCollapsed);
        }
        else if (heights.Count == 2)
        {
            States.Add(heights[0].Key, BottomSheetBehavior.StateExpanded);
            States.Add(heights[1].Key, BottomSheetBehavior.StateCollapsed);
        }
        else if (heights.Count == 3)
        {
            States.Add(heights[0].Key, BottomSheetBehavior.StateExpanded);
            States.Add(heights[1].Key, BottomSheetBehavior.StateHalfExpanded);
            States.Add(heights[2].Key, BottomSheetBehavior.StateCollapsed);
        }
    }

    internal int GetStateForDetent(Detent detent)
    {
        if (detent is null || !States.ContainsKey(detent))
        {
            return -1;
        }
        return States[detent];
    }
    internal Detent GetDetentForState(int state)
    {
        return States.FirstOrDefault(kv => kv.Value == state).Key;
    }

    public void Dismiss(bool animated)
    {

        if (animated)
        {
            windowContainer?.Backdrop.AnimateOut();
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
        frame.LayoutChange -= OnLayoutChange;
        windowContainer.RemoveFromParent();
    }

    public void Layout()
    {
        LayoutDetents(Heights, GetAvailableHeight());
    }

    internal void UpdateBackground()
    {
        Paint paint = _sheet.BackgroundBrush;
        if (Frame is not null)
        {
            if (_sheet.CornerRadius != -1)
            {
                SheetRadiusDrawable drawable;
                if (Frame.Background is not SheetRadiusDrawable)
                {
                    drawable = new SheetRadiusDrawable();
                    Frame.Background = drawable;
                }
                else
                {
                    drawable = (SheetRadiusDrawable)Frame.Background;
                }
                drawable.SetCornerRadius(Frame.Context.ToPixels(_sheet.CornerRadius));
            }
            if (paint is not null)
            {
                var platformColor = paint.ToColor().ToPlatform();
                if (Frame.Background is SheetRadiusDrawable sheetDrawable)
                {
                    sheetDrawable.SetColor(platformColor);
                }
                else
                {
                    Frame.BackgroundTintList = ColorStateList.ValueOf(platformColor);
                }
            }
        }
        // Try to find the background color to automatically change the status bar icons so they will
        // still be visible when the bottomsheet slides underneath the status bar.
        ColorStateList backgroundTint = ViewCompat.GetBackgroundTintList(Frame);

        if (backgroundTint != null)
        {
            // First check for a tint
            isBackgroundLight = MaterialColors.IsColorLight(backgroundTint.DefaultColor);
        }
        else if (Frame.Background is ColorDrawable)
        {
            // Then check for the background color
            isBackgroundLight = MaterialColors.IsColorLight(((ColorDrawable)Frame.Background).Color);
        }
        else
        {
            // Otherwise don't change the status bar color
            isBackgroundLight = null;
        }
    }

    public void UpdateHandleColor()
    {
        if (handle is null)
        {
            return;
        }
        if (_sheet.HandleColor is not null)
        {
            handle.SetColorFilter(_sheet.HandleColor.ToPlatform());
        }
    }

    static void EnsureStayOnFrontView(Context context)
    {
        if (stayOnFront is null || !stayOnFront.IsAttachedToWindow)
        {
            stayOnFront = new StayOnFrontView(context);
            var window = ((AppCompatActivity)context).Window;
            var parentView = window?.DecorView as ViewGroup;
            parentView.AddView(stayOnFront);
        }
    }

    void EnsureWindowContainer()
    {
        EnsureStayOnFrontView(_mauiContext.Context);
        if (windowContainer is null)
        {
            var container = (FrameLayout)AView.Inflate(_mauiContext.Context, Resource.Layout.the49_maui_bottom_sheet_design, null);

            container.ViewAttachedToWindow += ContainerAttachedToWindow;
            container.ViewDetachedFromWindow += ContainerDetachedFromWindow;

            windowContainer = new BottomSheetContainer(_mauiContext.Context, container);
            windowContainer.Backdrop.Click += BackdropClicked;

            frame = (FrameLayout)container.FindViewById(Resource.Id.design_bottom_sheet);

            frame.OutlineProvider = ViewOutlineProvider.Background;
            frame.ClipToOutline = true;

            ViewCompat.SetOnApplyWindowInsetsListener(windowContainer, new EdgeToEdgeListener(this));
            ViewCompat.SetWindowInsetsAnimationCallback(frame, new BottomSheetInsetsAnimationCallback(this));

            behavior = BottomSheetBehavior.From(frame);

            var callback = new BottomSheetCallback(_sheet);
            callback.StateChanged += Callback_StateChanged;
            behavior.AddBottomSheetCallback(callback);
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

    WindowInsetsCompat WindowInsets
    {
        get
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(23) && windowContainer.RootWindowInsets is not null)
            {
                return WindowInsetsCompat.ToWindowInsetsCompat(windowContainer.RootWindowInsets);
            }

            return WindowInsetsCompat.Consumed;
        }
    }

    Insets Insets
    {
        get
        {
            var insets = WindowInsets;
            if (OperatingSystem.IsAndroidVersionAtLeast(30))
            {
                return insets.GetInsetsIgnoringVisibility(Android.Views.WindowInsets.Type.SystemBars());
            }
#pragma warning disable CS0618
            return Insets.Of(insets.StableInsetLeft, insets.StableInsetTop, insets.StableInsetRight, insets.StableInsetBottom);
#pragma warning restore CS0618
        }
    }

    int KeyboardHeight => WindowInsets.GetInsets(WindowInsetsCompat.Type.Ime()).Bottom;
    int TopInset => Insets.Top;

    public double GetAvailableHeight()
    {
        var density = DeviceDisplay.MainDisplayInfo.Density;

        return (windowContainer.Height - TopInset - BottomInset - KeyboardHeight) / density;
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

        var keyboardHeight = KeyboardHeight;

        var top = sortedHeights[0].Value;

        // Configure the sheet to handle up to 3 detents

        if (sortedHeights.Count == 1)
        { // Only way to have one detent on Android is to use fitToContent. Use that
            behavior.FitToContents = true;
            behavior.SkipCollapsed = true;
        }
        else if (sortedHeights.Count == 2)
        { // We can handle a second detent by adding a collapsed state. Use peek height
            behavior.FitToContents = true;
            behavior.SkipCollapsed = false;

            var bottom = sortedHeights[1].Value;

            behavior.PeekHeight = (int)(bottom * density) + BottomInset + keyboardHeight;
        }
        else if (sortedHeights.Count == 3)
        { // 3 detents can be done using the peek height AND disabling fitToContent
          // Doing so uses a property called halfExpandedRatio, giving us
          // Expanded: Use ExpandedOffset to offset from the top
          // HalfExpanded: Use HalfExpandedRatio
          // Collapsed: Use PeekHeight

            behavior.FitToContents = false;
            behavior.SkipCollapsed = false;

            var midway = sortedHeights[1].Value;
            var bottom = sortedHeights[2].Value;

            // Set the top detent by offsetting the requested height from the maxHeight
            var topOffset = (maxSheetHeight - top) * density;
            behavior.ExpandedOffset = Math.Max(0, (int)topOffset);

            // Set the midway detent by calculating the ratio using the top detent info
            var ratio = ((midway * density) + keyboardHeight + BottomInset) / frame.LayoutParameters.Height;
            behavior.HalfExpandedRatio = (float)ratio;

            // Set the bottom detent using the peekHeight
            behavior.PeekHeight = (int)(bottom * density) + BottomInset + keyboardHeight;
        }
    }

    double CalculateTallestDetent(double heightConstraint)
    {
        if (Heights is null)
        {
            CalculateHeights(heightConstraint);
        }
        return Heights.Values.Max();
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

        var layoutParams = frame.LayoutParameters;

        layoutParams.Height = platformHeight + BottomInset + KeyboardHeight;

        if (height == maxHeight)
        {
            layoutParams.Height += TopInset;
        }
        _sheet.Arrange(new Rect(0, 0, frame.Width / density, height));
    }

    public void Show(bool animated)
    {
        isDuringShowingAnimation = true;

        EnsureWindowContainer();

        stayOnFront.AddView(windowContainer);

        frame.RemoveAllViews();

        // The Android view for the page could already have a ContainerView as a parent if it was shown as a bottom sheet before
        ((ContentViewGroup)_sheet.Handler?.PlatformView)?.RemoveFromParent();
        var containerView = _sheet.ToPlatform(_mauiContext);

        var c = new FrameLayout(_mauiContext.Context);

        if (_sheet.HasHandle)
        {
            handle = new BottomSheetDragHandleView(_mauiContext.Context);
            c.AddView(handle, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
        }

        c.AddView(containerView);

        frame.AddView(c);

        UpdateHasBackdrop();
        UpdateHandleColor();

        if (animated)
        {
            windowContainer?.Backdrop.AnimateIn();
            behavior.State = BottomSheetBehavior.StateHidden;
        }

        _sheet.Dispatcher.Dispatch(() =>
        {
            ResizeVirtualView();

            CalculateHeights(GetAvailableHeight());
            CalculateStates();
            Layout();
            UpdateBackground();

            var state = GetStateForDetent(_sheet.SelectedDetent);

            var defaultDetent = _sheet.GetDefaultDetent();
            if (state is -1)
            {
                state = Behavior.SkipCollapsed ? BottomSheetBehavior.StateExpanded : BottomSheetBehavior.StateCollapsed;
            }

            Behavior.State = state;

            containerView.LayoutChange += OnLayoutChange;

            _sheet.NotifyShowing();
        });
    }

    void OnLayoutChange(object sender, AView.LayoutChangeEventArgs e)
    {
        _sheet.Dispatcher.Dispatch(() =>
        {
            CalculateHeights(GetAvailableHeight());
            CalculateStates();
            ResizeVirtualView();
            Layout();
        });
    }

    void Callback_StateChanged(object sender, EventArgs e)
    {
        if (isDuringShowingAnimation && (
            Behavior.State == BottomSheetBehavior.StateCollapsed
            || Behavior.State == BottomSheetBehavior.StateHalfExpanded
            || Behavior.State == BottomSheetBehavior.StateExpanded
            ))
        {
            isDuringShowingAnimation = false;
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
        if (_sheet.SelectedDetent is null || Behavior is null || States is null)
        {
            return;
        }
        Behavior.State = GetStateForDetent(_sheet.SelectedDetent);
    }

    internal void UpdateHasBackdrop()
    {
        windowContainer?.SetBackdropVisibility(_sheet.HasBackdrop);
    }
}
