> **NOTE**: Coming from Gerald Versluis' video? Make sure to check the section on [what changed since the video was made](#changes-since-gerald-versluis-video)


<img src="./The49.Maui.BottomSheet.TitleLogo.svg?raw=true" height="64" />

## What is Maui.BottomSheet?

Maui.BottomSheet is a .NET MAUI library used to display pages as Bottom Sheets.

Android        |  iOS
:-------------------------:|:-------------------------:
<img src="screenshots/android.png?raw=true" height="480" />|<img src="screenshots/ios.png?raw=true" height="480" />


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
<the49:BottomSheet xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:the49="https://schemas.the49.com/dotnet/2023/maui"
             x:Class="MyApp.MySheetPage"
             Title="MySheetPage">
            <!-- ... -->
</the49:BottomSheet>
```

The sheet can be opened by calling the `Show(Window)` method of the page. It can be closed using `Dismiss()`:

```cs

const page = new MySheetPage();

// Pass the window in which the sheet should show. Usually accessible from any other page of the app.
page.Show(Window);

// Call to programatically close the sheet
page.Dismiss();

```

On Android, make sure your application's theme extends the Material3 theme. This mean you need a `Platforms/Android/Resources/values/styles.xml` file with the following content:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<resources>
	<style name="Maui.MainTheme" parent="Theme.Material3.DayNight"></style>
</resources>
```

If you already have this file, just make sure the `Maui.MainTheme` style inherits the `Theme.Material3.DayNight` parent.

## API

This library offers a `BottomSheetPage`, an extension of the `ContentPage` with extra functionality

### Properties

The following properties are available to use:

Name          |  Type | Default value | Description | Android | iOS |
:-------------------------|:-------------------------|---|:----|---|---|
`HasBackdrop` | `bool` | `false` | Displays the sheet as modal. This has no effect on whether or not the sheet can be dismissed using gestures. | ✅ | ❌* |
`HasHandle` | `bool` | `false` | If `true`, display a drag handle at the top of the sheet | ✅ | ✅ |
`IsCancelable` | `bool` | `true` | If `false`, prevents the dismissal of the sheet with user gestures | ✅ | ✅ |
`Detents` | `DetentsCollection` | `new DetentsCollection() { new ContentDetent() })` | A collection of detents where the sheet will snap to when dragged. (See the Detents section for more info) | ✅ | ✅ |

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

### Events

The following events are available to use:

Name          |  EventArg | Description | Android | iOS |
:-------------------------|:-------------------------|:----|---|---|
`Dismissed` | `DismissOrigin` | Invoked when the sheet is dismissed. The EventArgs will either be `DismissOrigin.Gesture` when the user dragged it down or `DismissOrigin.Programmatic` when `Dismiss` is called. | ✅ | ✅ |
`Showing` | `EventArg.Emtpy` | Called when the sheet is about to animate in. This is the best time to configure the behavior of the sheet for specific platforms (See [Platform specifics](#platform-specifics)) | ✅ | ✅ |


## Platform specifics

On Android, the `Google.Android.Material.BottomSheet.BottomSheetBehavior` is made available under `sheet.Controller.Behavior`, to ensure the property is set, access it when the `Showing` event is fired. Learn more about it here: [BottomSheetBehavior  |  Android Developers](https://developer.android.com/reference/com/google/android/material/bottomsheet/BottomSheetBehavior)

On iOS, the `UIKit.UISheetPresentationController` is made available under `sheet.Controller.SheetPresentationController`, to ensure the property is set, access it when the `Showing` event is fired. Learn more about it here: [UISheetPresentationController | Apple Developer Documentation](https://developer.apple.com/documentation/uikit/uisheetpresentationcontroller)

## Common questions

### How do I prevent the rounded corner to animate on Android?

```cs
var sheet = new MySheet();
sheet.Showing += (s, e) =>
{
    page.Controller.Behavior.DisableShapeAnimations();
};
sheet.Show(Window);
```

### How do I change the corner radius?

This will be different on Android and iOS as they each provide their own design implementation

On iOS

```cs
var sheet = new MySheet();
sheet.Showing += (s, e) =>
{
    sheet.Controller.SheetPresentationController.PreferredCornerRadius = 2;
};
sheet.Show(Window);
```

On Android (Using Android styles). In your `Platforms/Android/Resources/values/themes.xml` (or equivalent) add the following styles

```xml
<style name="ThemeOverlay.App.BottomSheetDialog" parent="ThemeOverlay.Material3.BottomSheetDialog">
    <item name="bottomSheetStyle">@style/ModalBottomSheetDialog</item>
</style>

<style name="ModalBottomSheetDialog" parent="Widget.Material3.BottomSheet.Modal">
    <item name="shapeAppearance">@style/ShapeAppearance.App.LargeComponent</item>
</style>

<style name="ShapeAppearance.App.LargeComponent" parent="ShapeAppearance.Material3.LargeComponent">
    <item name="cornerFamily">rounded</item>
    <item name="cornerSize">2dp</item>
</style>
```

And in your `<style name="Maui.MainTheme" ...>` add the following item:


```xml
<item name="bottomSheetDialogTheme">@style/ThemeOverlay.App.BottomSheetDialog</item>
```

## Implementation details

### iOS

The bottom sheet on iOS is presented using the `UIViewController`'s `PresentViewController` and configuring the sheet with [UISheetPresentationController](https://developer.apple.com/documentation/uikit/uisheetpresentationcontroller/detent).

Detents are created using the [custom](https://developer.apple.com/documentation/uikit/uisheetpresentationcontroller/detent/3976719-custom) method


### Android

The Material library's bottom sheet is used.

Standard sheets attach a `CoordinatorLayout` to the navigation view and insert a `FrameLayout` with the `com.google.android.material.bottomsheet.BottomSheetBehavior` Behavior.

Modal sheets use a [BottomSheetDialogFragment](https://developer.android.com/reference/com/google/android/material/bottomsheet/BottomSheetDialogFragment)

Detents are created using a combination of [expandedOffset](https://developer.android.com/reference/com/google/android/material/bottomsheet/BottomSheetBehavior#setExpandedOffset(int)), [halfExpandedRatio](https://developer.android.com/reference/com/google/android/material/bottomsheet/BottomSheetBehavior#setHalfExpandedRatio(float)) and [peekHeight](https://developer.android.com/reference/com/google/android/material/bottomsheet/BottomSheetBehavior#setPeekHeight(int,%20boolean)). These are the only configurable stop points for the bottom sheets, and that is why this library only supports up to 3 detents on Android.


## Changes since Gerald Versluis' video

If you're coming from [Gerald Versluis' video](https://www.youtube.com/watch?v=JJUm58avADo), a few things have changed. Here is what you need to know:

 - Property names have been updated to be more consistent, discoverable and aligned with standard MAUI properties:
   - `ShowHandle` is now `HasHandle`
   - `Cancelable` is now `IsCancelable`
   - `IsModal` is now `HasBackdrop`
 - Detents in XAML must be specified within a `DetentsCollection`


---


<img src="https://the49.com/logo.svg" height="64" />

Made within The49
