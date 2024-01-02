using The49.Maui.BottomSheet.Sample.DemoPages;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using Maui.BottomSheet.Sample.DemoPages;

namespace The49.Maui.BottomSheet.Sample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	public ObservableCollection<DemoEntry> Demos => new ObservableCollection<DemoEntry> {
		new DemoEntry
		{
			Title = "Chat demo",
			Description = "Use a sheet for a chat editor",
			Command = new Command(OpenChat),
		},
		new DemoEntry
		{
			Title = "Entry",
			Description = "Display any page as a bottom sheet",
			Command = new Command(OpenEntrySheet),
		},
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
			Title = "With custom handle color",
			Description = "Chose the color of the drag handle",
			Command = new Command(OpenHandleColorSheet),
		},
		new DemoEntry
		{
			Title = "Without animation",
			Description = "display the sheet immediately",
			Command = new Command(OpenNoAnimationSheet),
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
			Title = "Text sizing",
			Description = "Text should take as much space as it needs",
			Command = new Command(OpenTextSizing),
		},
		new DemoEntry
		{
			Title = "With ScrollView",
			Description = "Let the sheet expand, then scroll",
			Command = new Command(OpenScrollView),
		},
		new DemoEntry
		{
			Title = "Corner radius",
			Description = "specify a Corner radius for the sheet",
			Command = new Command(OpenCornerRadius),
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
		new DemoEntry
		{
			Title = "Selected Detent",
			Description = "control the selected detent",
			Command = new Command(OpenSelectedDetent),
		},
		new DemoEntry
		{
			Title = "Default detent",
			Description = "define a detent to be opened by default",
			Command = new Command(OpenDefaultDetent),
		},
		new DemoEntry
		{
			Title = "Open modal page",
			Description = "A sheet should behave correctly around opening a modal page",
			Command = new Command(OpenModalPage),
		},
		new DemoEntry
		{
			Title = "Sizing test",
			Description = "Check that the content is sized to the sheet",
			Command = new Command(OpenSizingTest),
		},
		new DemoEntry
		{
			Title = "Keyboard layout",
			Description = "Layout should update with keyboard",
			Command = new Command(OpenKeyboard),
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

	void OpenSimpleSheet()
	{
		var page = new SimplePage();
		page.ShowAsync(Window);
	}
	void OpenModalSheet()
	{
		var page = new SimplePage();
		page.HasBackdrop = true;
		page.ShowAsync(Window);
	}

	void OpenNotCancelableSheet()
	{
		var page = new SimplePage();
		page.IsCancelable = false;
		page.HasBackdrop = true;
		page.ShowAsync(Window);
	}
	void OpenHandleSheet()
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
		page.ShowAsync(Window);
	}
	void OpenHandleColorSheet()
	{
		var page = new SimplePage();
		page.HasBackdrop = true;
		page.HasHandle = true;
		page.HandleColor = Colors.Salmon;
		page.Detents = new DetentsCollection()
		{
			new FullscreenDetent(),
			new ContentDetent(),
			new AnchorDetent { Anchor = page.Divider },
		};
		page.ShowAsync(Window);
	}
	void OpenPeekableSheet()
	{
		var page = new SimplePage();
		page.Detents = new DetentsCollection()
		{
			new FullscreenDetent(),
			new ContentDetent(),
			new AnchorDetent { Anchor = page.Divider },
		};
		page.ShowAsync(Window);
	}

	async void OpenEntrySheet()
	{
		var sheet = new EntrySheet();

		await sheet.ShowAsync();
	}

	void OpenFullscreenSheet()
	{
		var page = new SimplePage();
		page.HasBackdrop = true;
		page.Detents = new DetentsCollection()
		{
			new FullscreenDetent(),
		};
		page.ShowAsync(Window);
	}
	void OpenBackgroundSheet()
	{
		var page = new SimplePage();
		page.Detents = new DetentsCollection()
		{
			new FullscreenDetent(),
			new ContentDetent(),
			new AnchorDetent { Anchor = page.Divider },
		};
		page.Background = Colors.Salmon;
		page.ShowAsync(Window);
	}
	void OpenCornerRadius()
	{
		var page = new SimplePage();
		page.Detents = new DetentsCollection()
		{
			new FullscreenDetent(),
			new ContentDetent(),
			new AnchorDetent { Anchor = page.Divider },
		};
		page.Background = Colors.Salmon;
		page.CornerRadius = 4;
		page.ShowAsync(Window);
	}
	void OpenRatioSheet()
	{
		var page = new SimplePage();
		page.Detents = new DetentsCollection()
		{
			new RatioDetent() { Ratio = .6f },
		};
		page.ShowAsync(Window);
	}

	void OpenHeightSheet()
	{
		var page = new SimplePage();
		page.Detents = new DetentsCollection()
		{
			new HeightDetent() { Height = 240 },
		};
		page.ShowAsync(Window);
	}

	void OpenTextSizing()
	{
		var p = new TextSheet();

		p.ShowAsync(Window);
	}

	void OpenDismissed()
	{
		var page = new SimplePage();
		page.HasBackdrop = true;
		page.Dismissed += (s, e) =>
		{
			DisplayAlert("Sheet was dismissed", e == DismissOrigin.Gesture ? "Sheet was dismissed by a user gesture" : "Sheet was dismissed programmatically", "close");
		};
		page.ShowAsync(Window);
	}

	void OpenSelectedDetent()
	{
		var page = new SimplePage();
		page.Detents = new DetentsCollection()
		{
			new FullscreenDetent(),
			new MediumDetent(),
			new RatioDetent { Ratio = .2f },
		};
		page.HasBackdrop = false;
		page.PropertyChanged += (s, e) =>
		{
			if (e.PropertyName == nameof(page.SelectedDetent))
			{
				Toast.Make($"Selected Detent is now {(page.SelectedDetent is null ? "unknown" : page.SelectedDetent.ToString())}").Show();
			}
		};
		page.SetExtraContent(new HorizontalStackLayout
		{
			new Button { Text = "small", Command = new Command(() => page.SelectedDetent = page.Detents[2]) },
			new Button { Text = "medium", Command = new Command(() => page.SelectedDetent = page.Detents[1]) },
			new Button { Text = "large", Command = new Command(() => page.SelectedDetent = page.Detents[0]) },
		});
		page.ShowAsync(Window);
	}

	void OpenDefaultDetent()
	{
		var page = new SimplePage();
		page.Detents = new DetentsCollection()
		{
			new FullscreenDetent(),
			new MediumDetent() { IsDefault = true },
			new RatioDetent { Ratio = .2f },
		};
		page.HasBackdrop = false;
		page.ShowAsync(Window);
	}

	void OpenNoAnimationSheet()
	{
		var page = new SimplePage();
		page.Detents = new DetentsCollection()
		{
			new FullscreenDetent(),
			new ContentDetent(),
		};
		page.HasBackdrop = true;
		page.SetExtraContent(new Button { Text = "Dismiss without animation", Command = new Command(() => page.DismissAsync(false)) });
		page.ShowAsync(Window, false);
	}

	void OpenScrollView()
	{
		var sheet = new ScrollSheet();

		sheet.ShowAsync(Window);
	}

	void OpenModalPage()
	{
		var page = new SimplePage();
		page.Detents = new DetentsCollection()
		{
			new FullscreenDetent(),
			new ContentDetent(),
		};
		page.HasBackdrop = true;
		var b = new Button
		{
			Text = "Go to page"
		};

		var g = new TapGestureRecognizer
		{
			Command = new Command(() =>
			{
				page.DismissAsync(false);
				Shell.Current.GoToAsync("//ModalPage");
			}),
		};

		b.GestureRecognizers.Add(g);
		page.SetExtraContent(b);
		page.ShowAsync(Window);
	}

	void OpenSizingTest()
	{
		var t = new SizingTest();

		t.ShowAsync(Window);
	}

	void OpenKeyboard()
	{
		var t = new EntrySheet();
		t.Detents = new DetentsCollection()
		{
			new FullscreenDetent(),
			new MediumDetent(),
			new ContentDetent(),
		};
		t.ShowAsync(Window);
	}

	void OpenChat()
	{
		Shell.Current.Navigation.PushAsync(new ChatPage());
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
		page.ShowAsync(Window);
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
        page.ShowAsync(Window);
    }
#endif

	void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

	void Button_Clicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//ModalPage");
	}
}

