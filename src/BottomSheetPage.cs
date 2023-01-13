namespace The49.Maui.BottomSheet;

public enum BottomSheetPageState
{
    Hidden,
    Peeking,
    HalfExpanded,
    Expanded,
    Dragging,
}

public partial class BottomSheetPage : ContentPage
{
    public static readonly BindableProperty DetentsProperty = BindableProperty.Create(nameof(Detents), typeof(DetentsCollection), typeof(BottomSheetPage), new DetentsCollection() { new ContentDetent() });
    public static readonly BindableProperty IsModalProperty = BindableProperty.Create(nameof(IsModal), typeof(bool), typeof(BottomSheetPage), false);
    public static readonly BindableProperty ShowHandleProperty = BindableProperty.Create(nameof(ShowHandle), typeof(bool), typeof(BottomSheetPage), false);
    public static readonly BindableProperty CancelableProperty = BindableProperty.Create(nameof(Cancelable), typeof(bool), typeof(BottomSheetPage), true);

    //public event EventHandler<float> Sliding;

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

    internal static BottomSheetPageState GetDefaultOpeningState(bool isPeekable, bool isFullScreen)
    {
        if (isPeekable)
        {
            return BottomSheetPageState.Peeking;
        }
        if (isFullScreen)
        {
            return BottomSheetPageState.HalfExpanded;
        }
        return BottomSheetPageState.Expanded;
    }

    public BottomSheetPage(): base()
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

    public void Dismiss()
    {
        Handler?.Invoke(nameof(Dismiss));
    }

    internal List<Detent> GetEnabledDetents()
    {
        return Detents.Where(d => d.IsEnabled).ToList();
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
