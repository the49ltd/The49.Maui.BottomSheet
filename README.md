# <img src="./The49.Maui.BottomSheet.icon.svg?raw=true" height="24" alt="The49.Maui.BottomSheet icon" /> The49.Maui.BottomSheet

[![NuGet Version](https://img.shields.io/nuget/v/The49.Maui.BottomSheet)](https://www.nuget.org/packages/The49.Maui.BottomSheet)

> **NOTE**: Coming from Gerald Versluis' video? Make sure to check the section on [what changed since the video was made](#changes-since-gerald-versluis-video)




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

Simply create a `ContentView`. Replace the extended class with `BottomSheet` in code-behind and in XAML:

```cs
using The49.Maui.BottomSheet;

public class MySheet : BottomSheet
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
             x:Class="MyApp.MySheet"
             Title="MySheet">
            <!-- ... -->
</the49:BottomSheet>
```

The sheet can be opened by calling the `ShowAsync()` method of the page. It can be closed using `DismissAsync()`:

```cs

const sheet = new MySheet();

// Show the sheet
sheet.ShowAsync();

// Alternatively, pass the window in which the sheet should show. Usually accessible from any other page of the app.
sheet.ShowAsync(Window);

// Call to programatically close the sheet
sheet.DismissAsync();

```

An extra parameter can be passed to both `ShowAsync` and `DismissAsync` to enable/disable the animations.

On Android, make sure your application's theme extends the Material3 theme. This mean you need a `Platforms/Android/Resources/values/styles.xml` file with the following content:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<resources>
	<style name="Maui.MainTheme" parent="Theme.Material3.DayNight"></style>
</resources>
```

If you already have this file, just make sure the `Maui.MainTheme` style inherits the `Theme.Material3.DayNight` parent.

## API

This library offers a `BottomSheet`, an extension of the `ContentView` with extra functionality

### Properties

The following properties are available to use:

Name          |  Type | Default value | Description | Android | iOS |
:-------------------------|:-------------------------|---|:----|---|---|
`HasBackdrop` | `bool` | `false` | Displays the sheet as modal. This has no effect on whether or not the sheet can be dismissed using gestures. | ✅ | ✅* |
`HasHandle` | `bool` | `false` | If `true`, display a drag handle at the top of the sheet | ✅ | ✅ |
`HandleColor` | `Color` | `null` | Sets the color of the sheet's handle is `HasHandle` is true | ✅ | ❌** |
`CornerRadius` | `double` | `-1` | Sets the sheet's corner radius | ✅*** | ✅ |
`IsCancelable` | `bool` | `true` | If `false`, prevents the dismissal of the sheet with user gestures | ✅ | ✅ |
`Detents` | `DetentsCollection` | `new DetentsCollection() { new ContentDetent() })` | A collection of detents where the sheet will snap to when dragged. (See the Detents section for more info) | ✅ | ✅ |
`SelectedDetent` | `Detent` | `null` | A two way property defining which detent is currently selected. Changes as the user slides, and updates the sheet's position when changed | ✅ | ✅* |

\* iOS doesn't support the property `largestUndimmedDetentIdentifier` or `selectedDetentIdentifer` for custom detents as of right now. Se iOS documentation for [largestUndimmedDetentIdentifier](https://developer.apple.com/documentation/uikit/uisheetpresentationcontroller/3858107-largestundimmeddetentidentifier) and [selectedDetentIdentifer](https://developer.apple.com/documentation/uikit/uisheetpresentationcontroller/3801908-selecteddetentidentifier)
Only when the `FullscreenDetent` and/or `MediumDetent` are used those properties will work.

\*\* iOS doesn't give any access to the grabber view

\*\*\* On Android, setting a custom corner radius will prevent the radius animation when the sheet hits the top of the screen

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
`MediumDetent` |  | A detent at the halfway point of the screen |

Example:

```xml
<the49:BottomSheet xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:the49="https://schemas.the49.com/dotnet/2023/maui"
             x:Class="MyApp.SheetPage"
             Title="SheetPage">
    <the49:BottomSheet.Detents>
        <!-- Stop at the height of the screen -->
        <the49:FullscreenDetent />
        <!-- Stop at the height of the page content -->
        <the49:ContentDetent />
        <!-- Stop at 120dp -->
        <the49:HeightDetent Height="120" />
        <!-- Stop at 45% of the screen height -->
        <the49:RatioDetent Ratio="0.45" />
        <!-- Stop at the height of the divider view -->
        <the49:AnchorDetent Anchor="{x:Reference divider}" />
    </the49:BottomSheet.Detents>
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
`Shown` | `EventArg.Emtpy` | Called when the sheet finished animating in. | ✅ | ✅ |


## Platform specifics

On Android, the `Google.Android.Material.BottomSheet.BottomSheetBehavior` is made available under `sheet.Controller.Behavior`, to ensure the property is set, access it when the `Showing` event is fired. Learn more about it here: [BottomSheetBehavior  |  Android Developers](https://developer.android.com/reference/com/google/android/material/bottomsheet/BottomSheetBehavior)

On iOS, the `UIKit.UISheetPresentationController` is made available under `sheet.Controller.SheetPresentationController`, to ensure the property is set, access it when the `Showing` event is fired. Learn more about it here: [UISheetPresentationController | Apple Developer Documentation](https://developer.apple.com/documentation/uikit/uisheetpresentationcontroller)

## Common questions

### How can I remove the backdrop on iOS

A sheet without backdrop works on iOS only if using `MediumDetent` and `FullscreenDetent`. Using the `OnPlatform` tool replace the detent on iOS to only use those and it will work.

> Note: In the future, iOS might allow custom detents to support the largestUndimmableDetentIdentifier. If that happens, this plugin will be updated to support it

Here is how you can use the detents you need on Android, and use the detents compatible with `HasBackdrop` and `SelectedDetent`:

A `IsDefault` property can be used to select the detent that will be shown when calling `ShowAsync`. Otherwise the smallest detent is used.

```xml
<the49:BottomSheet xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:the49="https://schemas.the49.com/dotnet/2023/maui"
             x:Class="MyApp.SheetPage"
             Title="SheetPage">
    <the49:BottomSheet.Detents>
        <OnPlatform>
            <On Platform="Android">
                <the49:FullscreenDetent />
                <the49:ContentDetent />
                <the49:AnchorDetent Anchor="{x:Reference divider}" />
            </On>
            <On Platform="iOS">
                <the49:FullscreenDetent />
                <the49:MediumDetent />
            </On>
        </OnPlatform>
    </the49:BottomSheet.Detents>
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

### How do I prevent the rounded corner to animate on Android?

```cs
var sheet = new MySheet();
sheet.Showing += (s, e) =>
{
    page.Controller.Behavior.DisableShapeAnimations();
};
sheet.ShowAsync(Window);
```
### How can I change the detent used when showing the sheet

You can either add `IsDefault="True"` to the detent or set `SelectedDetent` to one of your detents before calling `ShowAsync`.

### How do I change the corner radius?

Use the `CornerRadius` property.

## Implementation details

### iOS

The bottom sheet on iOS is presented using the `UIViewController`'s `PresentViewController` and configuring the sheet with [UISheetPresentationController](https://developer.apple.com/documentation/uikit/uisheetpresentationcontroller/detent).

Detents are created using the [custom](https://developer.apple.com/documentation/uikit/uisheetpresentationcontroller/detent/3976719-custom) method


### Android

The Material library's bottom sheet is used.

A xml layout contains a Frame, CoordinatorLayout and another Frame with the `com.google.android.material.bottomsheet.BottomSheetBehavior` behavior. The layout is inflated and added to the `DrawerLayout` is using `AppShell` or `CoordinatorLayout` is using `NavigationPage`

A backdrop is added and animated when requested

Detents are created using a combination of [expandedOffset](https://developer.android.com/reference/com/google/android/material/bottomsheet/BottomSheetBehavior#setExpandedOffset(int)), [halfExpandedRatio](https://developer.android.com/reference/com/google/android/material/bottomsheet/BottomSheetBehavior#setHalfExpandedRatio(float)) and [peekHeight](https://developer.android.com/reference/com/google/android/material/bottomsheet/BottomSheetBehavior#setPeekHeight(int,%20boolean)). These are the only configurable stop points for the bottom sheets, and that is why this library only supports up to 3 detents on Android.


## Changes since Gerald Versluis' video

If you're coming from [Gerald Versluis' video](https://www.youtube.com/watch?v=JJUm58avADo), a few things have changed. Here is what you need to know:

 - Property names have been updated to be more consistent, discoverable and aligned with standard MAUI properties:
   - `ShowHandle` is now `HasHandle`
   - `Cancelable` is now `IsCancelable`
   - `IsModal` is now `HasBackdrop`

 - 2 new properties have been added:
   - `HandleColor`
   - `SelectedDetent`
 - Methods have been renamed
   - `Show` is now `ShowAsync` and completes when the animation of the sheet finishes. It also accepts a boolean to turn off animations
   - `Dismiss` is now `DismissAsync` and completes when the animation of the sheet finishes. It also accepts a boolean to turn off animations


---


<img src="https://the49.com/logo.svg" height="64" />

Made within The49
