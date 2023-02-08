namespace The49.Maui.BottomSheet;

public enum BottomSheetState
{
    Hidden,
    Peeking,
    HalfExpanded,
    Expanded,
    Dragging,
}

public enum DismissOrigin
{
    Gesture,
    Programmatic,
}

public partial class BottomSheet : ContentView
{
    public static readonly BindableProperty DetentsProperty = BindableProperty.Create(nameof(Detents), typeof(DetentsCollection), typeof(BottomSheet), new DetentsCollection() { new ContentDetent() });
    public static readonly BindableProperty IsModalProperty = BindableProperty.Create(nameof(IsModal), typeof(bool), typeof(BottomSheet), false);
    public static readonly BindableProperty ShowHandleProperty = BindableProperty.Create(nameof(ShowHandle), typeof(bool), typeof(BottomSheet), false);
    public static readonly BindableProperty CancelableProperty = BindableProperty.Create(nameof(Cancelable), typeof(bool), typeof(BottomSheet), true);

    //public event EventHandler<float> Sliding;
    public event EventHandler<DismissOrigin> Dismissed;

    DismissOrigin _dismissOrigin = DismissOrigin.Gesture;

    public DetentsCollection Detents
    {
        get => (DetentsCollection)GetValue(DetentsProperty);
        set => SetValue(DetentsProperty, value);
    }

    public bool IsModal
    {
        get => (bool)GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    public bool ShowHandle
    {
        get => (bool)GetValue(ShowHandleProperty);
        set => SetValue(ShowHandleProperty, value);
    }

    public bool Cancelable
    {
        get => (bool)GetValue(CancelableProperty);
        set => SetValue(CancelableProperty, value);
    }

    internal static BottomSheetState GetDefaultOpeningState(bool isPeekable, bool isFullScreen)
    {
        if (isPeekable)
        {
            return BottomSheetState.Peeking;
        }
        if (isFullScreen)
        {
            return BottomSheetState.HalfExpanded;
        }
        return BottomSheetState.Expanded;
    }

    public BottomSheet() : base()
    {
        Resources.Add(new Style(typeof(Label)));
    }

    public override SizeRequest Measure(double widthConstraint, double heightConstraint, MeasureFlags flags = MeasureFlags.None)
    {
        var r = Content.Measure(widthConstraint, heightConstraint, flags);

        return new SizeRequest(
            new Size(r.Request.Width + Padding.HorizontalThickness, r.Request.Height + Padding.VerticalThickness),
            new Size(r.Minimum.Width + Padding.HorizontalThickness, r.Minimum.Height + Padding.VerticalThickness)
        );
    }

    public void Show(Window window)
    {
        BottomSheetManager.Show(window, this);
    }

    public Task Dismiss()
    {
        _dismissOrigin = DismissOrigin.Programmatic;
        var completionSource = new TaskCompletionSource();
        void OnDismissed(object sender, DismissOrigin origin)
        {
            Dismissed -= OnDismissed;
            completionSource.SetResult();
        }
        Dismissed += OnDismissed;
        Handler?.Invoke(nameof(Dismiss));
        return completionSource.Task;
    }

    internal List<Detent> GetEnabledDetents()
    {
        return Detents.Where(d => d.IsEnabled).ToList();
    }

    internal void NotifyDismissed()
    {
        Dismissed?.Invoke(this, _dismissOrigin);
    }

    internal Brush? BackgroundBrush
    {
        get
        {
            if (!Background.IsEmpty)
            {
                return Background;
            }
            if (BackgroundColor.IsNotDefault())
            {
                return new SolidColorBrush(BackgroundColor);
            }
            return null;
        }
    }
}
