## What is Maui.BottomSheet?

Maui.BottomSheet is a .NET MAUI library used to display pages as Bottom Sheets.

## Setup

Enable this plugin by calling `UseBottomSheet()` in your `MauiProgram.cs`


```cs
using Maui.Insets;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		
		// Initialise the plugin
		builder
            .UseMauiApp<App>()
            .UseBottomSheet();

		// the rest of your logic...
	}
}
```

### XAML usage

In order to make use of the plugin within XAML you can use this namespace:

```xml
xmlns:the49="https://schemas.the49.com/dotnet/2023/maui"
```

### Quick usage

Simply create a `ContentPage`. Replace the extended class with `BottomSheetPage` in code-behind and in XAML:

```cs
using The49.Maui.BottomSheet;

public class MySheetPage : BottomSheetPage
{
    public MySheetPage()
    {
        InitializeComponent();
    }
}
```


```xml
<the49:BottomSheetPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:the49="https://schemas.the49.com/dotnet/2023/maui"
             x:Class="MyApp.MySheetPage"
             Title="MySheetPage">
            <!-- ... -->
</the49:BottomSheetPage>
```

The sheet can be opened by calling the `Show(Window)` method of the page. It can be closed using `Dismiss()`:

```cs

const page = new MySheetPage();

// Pass the window in which the sheet should show. Usually accessible from any other page of the app.
page.Show(Window);

// Call to programatically close the sheet
page.Dismiss();

```

## API

This library offers a `BottomSheetPage`, an extension of the `ContentPage` with extra functionality

### Properties

The following properties are available to use:

Name          |  Type | Default value | Description | Android | iOS |
:-------------------------|:-------------------------|---|:----|---|---|
IsModal | `bool` | `false` | Displays the sheet as modal. This has no effect on whether or not the sheet can be dismissed using gestures. | ✅ | ❌* |
ShowHandle | `bool` | `false` | If true, display a drag handle at the top of the sheet | ✅ | ✅ |
Cancelable | `bool` | `true` | If true, prevents the dismissal of the sheet with user gestures | ✅ | ✅ |
Detents | `DetentsCollection` | `new DetentsCollection() { new ContentDetent() })` | A collection of detents where the sheet will snap to when dragged. (See the Detents section for more info) | ✅ | ✅ |

\* iOS doesn't support the property `largestUndimmedDetentIdentifier` for custom detents as of right now. [See iOS documentation](https://developer.apple.com/documentation/uikit/uisheetpresentationcontroller/3858107-largestundimmeddetentidentifier)

### Detents:

Detents are snap point at which the sheet will stop when a drag gesture is released [See iOS definition](https://developer.apple.com/documentation/uikit/uisheetpresentationcontroller/detent).

On Android only 3 detents are supported (See implemenation section for more info).

On iOS, detents are only fully supported for iOS 16 and up. On iOS 15, medium and large detents are used instead [See iOS documentation](https://developer.apple.com/documentation/uikit/uisheetpresentationcontroller/detent).

### Available detents

Name          |  Parameter | Description |
:-------------------------|:-------------------------|:----|
`FullscreenDetent` |  | Full height of the screen |
`ContentDetent` |  | Calculates the height of the page's content |
`AnchorDetent` | `Anchor` | `Anchor` expects a `View` and will set its height to the Y position of that view. This is used to peek some content, then reveal more when the sheet is dragged up |
`HeightDetent` | `Height` | Use a dp value to specify the detent height |
`RatioDetent` | `Ratio` | Use a ratio of the full screen height |

Example:

```xml
<the49:BottomSheetPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:the49="https://schemas.the49.com/dotnet/2023/maui"
             x:Class="MyApp.SheetPage"
             Title="SheetPage">
    <the49:BottomSheetPage.Detents>
        <the49:DetentsCollection>
            <!-- Stop at the height of the screen -->
            <the49:FullscreenDetent />
            <!-- Stop at the height of the page content -->
            <the49:ContentDetent />
            <!-- Stop at 120dp -->
            <the49:HeightDetent Height="120" />
            <!-- Stop at 45% of the screen height -->
            <the49:RatioDetent Height="0.45" />
            <!-- Stop at the height of the divider view -->
            <the49:AnchorDetent Anchor="{x:Reference divider}" />
        </the49:DetentsCollection>
    </the49:BottomSheetPage.Detents>
    <VerticalStackLayout Spacing="16">
        <VerticalStackLayout>
            <!-- some content -->
        </VerticalStackLayout>
        <BoxView x:Name="divider" />
        <VerticalStackLayout>
            <!-- more content -->
        </VerticalStackLayout>
    </VerticalStackLayout>
</the49:BottomSheetPage>
```

### Custom detent

You can create a custom detent by extending the default `Detent` class and implementing its `GetHeight` abstract method

## Implementation details

### iOS

The bottom sheet on iOS is presented using the `UIViewController`'s `PresentViewController` and configuring the sheet with [UISheetPresentationController](https://developer.apple.com/documentation/uikit/uisheetpresentationcontroller/detent).

Detents are created using the [custom](https://developer.apple.com/documentation/uikit/uisheetpresentationcontroller/detent/3976719-custom) method


### Android

The Material library's bottom sheet is used.

Standard sheets attach a `CoordinatorLayout` to the navigation view and insert a `FrameLayout` with the `com.google.android.material.bottomsheet.BottomSheetBehavior` Behavior.

Modal sheets use a [BottomSheetDialogFragment](https://developer.android.com/reference/com/google/android/material/bottomsheet/BottomSheetDialogFragment)

Detents are created using a combination of [expandedOffset](https://developer.android.com/reference/com/google/android/material/bottomsheet/BottomSheetBehavior#setExpandedOffset(int)), [halfExpandedRatio](https://developer.android.com/reference/com/google/android/material/bottomsheet/BottomSheetBehavior#setHalfExpandedRatio(float)) and [peekHeight](https://developer.android.com/reference/com/google/android/material/bottomsheet/BottomSheetBehavior#setPeekHeight(int,%20boolean)). These are the only configurable stop points for the bottom sheets, and that is why this library only supports up to 3 detents on Android.



---

Made within The49
