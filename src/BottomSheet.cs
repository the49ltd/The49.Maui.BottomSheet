#nullable enable

namespace The49.Maui.BottomSheet;

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
    public static readonly BindableProperty HandleColorProperty = BindableProperty.Create(nameof(HandleColor), typeof(Color), typeof(BottomSheet), null);
    public static readonly BindableProperty IsCancelableProperty = BindableProperty.Create(nameof(IsCancelable), typeof(bool), typeof(BottomSheet), true);
    public static readonly BindableProperty SelectedDetentProperty = BindableProperty.Create(nameof(SelectedDetent), typeof(Detent), typeof(BottomSheet), null, BindingMode.TwoWay);

    //public event EventHandler<float> Sliding;
    public event EventHandler<DismissOrigin>? Dismissed;
    public event EventHandler? Showing;
    public event EventHandler? Shown;

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

    public Color? HandleColor
    {
        get => (Color?)GetValue(HandleColorProperty);
        set => SetValue(HandleColorProperty, value);
    }

    public bool IsCancelable
    {
        get => (bool)GetValue(IsCancelableProperty);
        set => SetValue(IsCancelableProperty, value);
    }

    public Detent? SelectedDetent
    {
        get => (Detent?)GetValue(SelectedDetentProperty);
        set => SetValue(SelectedDetentProperty, value);
    }

    public BottomSheet() : base()
    {
        Resources.Add(new Style(typeof(Label)));
    }

    public Task ShowAsync(bool animated = true)
    {
        var window = Application.Current?.Windows[0];
        if (window is null)
        {
            return Task.CompletedTask;
        }
        return ShowAsync(window, animated);
    }

    public Task ShowAsync(Window window, bool animated = true)
    {
        var completionSource = new TaskCompletionSource();
        void OnShown(object? sender, EventArgs e)
        {
            Shown -= OnShown;
            completionSource.SetResult();
        }
        Shown += OnShown;

        if (SelectedDetent is null)
        {
            SelectedDetent = GetDefaultDetent();
        }

        BottomSheetManager.Show(window, this, animated);
        return completionSource.Task;
    }

    public Task DismissAsync(bool animated = true)
    {
        _dismissOrigin = DismissOrigin.Programmatic;
        var completionSource = new TaskCompletionSource();
        void OnDismissed(object? sender, DismissOrigin origin)
        {
            Dismissed -= OnDismissed;
            completionSource.SetResult();
        }
        Dismissed += OnDismissed;
        Handler?.Invoke(nameof(DismissAsync), animated);
        return completionSource.Task;
    }

    internal IEnumerable<Detent> GetEnabledDetents()
    {
        var enabledDetents = Detents.Where(d => d.IsEnabled);

        if (enabledDetents.Count() == 0)
        {
            return new List<Detent> { new ContentDetent() };
        }
        return enabledDetents;
    }

    internal Detent? GetDefaultDetent()
    {
        var detents = GetEnabledDetents();
        var detent = SelectedDetent;
        if (SelectedDetent is not null)
        {
            return SelectedDetent;
        }
        return detents.FirstOrDefault(d => d.IsDefault);
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
    internal void NotifyShown()
    {
        Shown?.Invoke(this, EventArgs.Empty);
    }
}
