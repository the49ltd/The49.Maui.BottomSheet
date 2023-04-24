using The49.Maui.BottomSheet.DemoPages;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace The49.Maui.BottomSheet;

public class DemoEntry
{
    public string Title { get; set; }
    public string Description { get; set; }
    public ICommand Command { get; set; }
}

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    public ObservableCollection<DemoEntry> Demos => new ObservableCollection<DemoEntry> {
        new DemoEntry
        {
            Title = "Non-modal sheet",
            Description = "Display any page as a bottom sheet",
            Command = new Command(OpenSimpleSheet),
        },
        new DemoEntry
        {
            Title = "Modal sheet",
            Description = "Display the sheet as modal",
            Command = new Command(OpenModalSheet),
        },
        new DemoEntry
        {
            Title = "No sliding away",
            Description = "The sheet cannot be closed by sliding the sheet",
            Command = new Command(OpenNotCancelableSheet),
        },
        new DemoEntry
        {
            Title = "With handle",
            Description = "Display the drag handle",
            Command = new Command(OpenHandleSheet),
        },
        new DemoEntry
        {
            Title = "Specify a height",
            Description = "Use a dp value at which the sheet will open",
            Command = new Command(OpenHeightSheet),
        },
        new DemoEntry
        {
            Title = "Specify a ratio between 0 and 1",
            Description = "The sheet will open at the position specified",
            Command = new Command(OpenRatioSheet),
        },
        new DemoEntry
        {
            Title = "Anchor a detent to a specific view",
            Description = "The sheet will snap to a position hiding the anchor",
            Command = new Command(OpenPeekableSheet),
        },
        new DemoEntry
        {
            Title = "Fullscreen",
            Description = "Use the full height of the screen",
            Command = new Command(OpenFullscreenSheet),
        },
        new DemoEntry
        {
            Title = "Background color",
            Description = "specify a BackgroundColor for the sheet",
            Command = new Command(OpenBackgroundSheet),
        },
        new DemoEntry
        {
            Title = "Dismissed",
            Description = "listen to the dismissed event",
            Command = new Command(OpenDismissed),
        },
#if ANDROID
        new DemoEntry
        {
            Title = "Customize behavior",
            Description = "access the Android BottomSheetBehavior",
            Command = new Command(OpenCustomizeBehavior),
        },
#elif IOS
        new DemoEntry
        {
            Title = "Customize behavior",
            Description = "access the iOS UISheetPresentationControllerDelegate",
            Command = new Command(OpenCustomizeBehavior),
        },
#endif
    };

    private void OpenSimpleSheet()
    {
        var page = new SimplePage();
        page.Show(Window);
    }
    private void OpenModalSheet()
    {
        var page = new SimplePage();
        page.HasBackdrop = true;
        page.Show(Window);
    }
    private void OpenNotCancelableSheet()
    {
        var page = new SimplePage();
        page.IsCancelable = false;
        page.HasBackdrop = true;
        page.Show(Window);
    }
    private void OpenHandleSheet()
    {
        var page = new SimplePage();
        page.HasBackdrop = true;
        page.HasHandle = true;
        page.Detents = new DetentsCollection()
        {
            new FullscreenDetent(),
            new ContentDetent(),
            new AnchorDetent { Anchor = page.Divider },
        };
        page.Show(Window);
    }
    private void OpenPeekableSheet()
    {
        var page = new SimplePage();
        page.Detents = new DetentsCollection()
        {
            new FullscreenDetent(),
            new ContentDetent(),
            new AnchorDetent { Anchor = page.Divider },
        };
        page.Show(Window);
    }
    private void OpenFullscreenSheet()
    {
        var page = new SimplePage();
        page.HasBackdrop = true;
        page.Detents = new DetentsCollection()
        {
            new FullscreenDetent(),
        };
        page.Show(Window);
    }
    private void OpenBackgroundSheet()
    {
        var page = new SimplePage();
        page.Detents = new DetentsCollection()
        {
            new FullscreenDetent(),
            new ContentDetent(),
            new AnchorDetent { Anchor = page.Divider },
        };
        page.Background = Colors.Salmon;
        page.Show(Window);
    }
    private void OpenRatioSheet()
    {
        var page = new SimplePage();
        page.Detents = new DetentsCollection()
        {
            new RatioDetent() { Ratio = .6f },
        };
        page.Show(Window);
    }

    private void OpenHeightSheet()
    {
        var page = new SimplePage();
        page.Detents = new DetentsCollection()
        {
            new HeightDetent() { Height = 240 },
        };
        page.Show(Window);
    }

    void OpenDismissed()
    {
        var page = new SimplePage();
        page.HasBackdrop = true;
        page.Dismissed += (s, e) =>
        {
            DisplayAlert("Sheet was dismissed", e == DismissOrigin.Gesture ? "Sheet was dismissed by a user gesture" : "Sheet was dismissed programmatically", "close");
        };
        page.Show(Window);
    }

#if ANDROID
    void OpenCustomizeBehavior()
    {
        var page = new SimplePage();
        page.HasBackdrop = true;
        page.Showing += (s, e) =>
        {
            page.Controller.Behavior.DisableShapeAnimations();
        };
        page.Show(Window);
    }
#elif IOS
    void OpenCustomizeBehavior()
    {
        var page = new SimplePage();
        page.HasBackdrop = true;
        page.Showing += (s, e) =>
        {
            page.Controller.SheetPresentationController.PreferredCornerRadius = 2;
        };
        page.Show(Window);
    }
#endif

    private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = (DemoEntry)list.SelectedItem;
        if (item == null)
        {
            return;
        }
        item.Command.Execute(null);
    }

    void list_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        Header.TranslationY = Math.Max(-e.VerticalOffset, -72);
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var item = (DemoEntry)((BindableObject)sender).BindingContext;
        if (item == null)
        {
            return;
        }
        item.Command.Execute(null);
    }
}

