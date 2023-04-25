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
    public static readonly BindableProperty DetentsProperty = BindableProperty.Create(nameof(Detents), typeof(IList<Detent>), typeof(BottomSheet), default(IList<Detent>),
        defaultValueCreator: bindable =>
        {
            return new List<Detent>();
        });
    public static readonly BindableProperty HasBackdropProperty = BindableProperty.Create(nameof(HasBackdrop), typeof(bool), typeof(BottomSheet), false);
    public static readonly BindableProperty HasHandleProperty = BindableProperty.Create(nameof(HasHandle), typeof(bool), typeof(BottomSheet), false);
    public static readonly BindableProperty IsCancelableProperty = BindableProperty.Create(nameof(IsCancelable), typeof(bool), typeof(BottomSheet), true);

    //public event EventHandler<float> Sliding;
    public event EventHandler<DismissOrigin> Dismissed;
    public event EventHandler Showing;

    DismissOrigin _dismissOrigin = DismissOrigin.Gesture;

    public IList<Detent> Detents
    {
        get => (IList<Detent>)GetValue(DetentsProperty);
        set => SetValue(DetentsProperty, value);
    }

    public bool HasBackdrop
    {
        get => (bool)GetValue(HasBackdropProperty);
        set => SetValue(HasBackdropProperty, value);
    }

    public bool HasHandle
    {
        get => (bool)GetValue(HasHandleProperty);
        set => SetValue(HasHandleProperty, value);
    }

    public bool IsCancelable
    {
        get => (bool)GetValue(IsCancelableProperty);
        set => SetValue(IsCancelableProperty, value);
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
        return new SizeRequest(
            new Size(widthConstraint, heightConstraint),
            new Size(widthConstraint, heightConstraint)
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

    internal IEnumerable<Detent> GetEnabledDetents()
    {
        return Detents.Where(d => d.IsEnabled);
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

    internal void NotifyShowing()
    {
        Showing?.Invoke(this, EventArgs.Empty);
    }
}
