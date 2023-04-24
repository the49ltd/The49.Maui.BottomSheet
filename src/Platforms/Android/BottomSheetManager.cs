using Android.Views;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;
using Google.Android.Material.BottomSheet;
using Android.Widget;

namespace The49.Maui.BottomSheet;

internal partial class BottomSheetManager
{
    static partial void PlatformShow(Microsoft.Maui.Controls.Window window, BottomSheet page)
    {
        page.Parent = window;
        IBottomSheetController controller;
        if (page.HasBackdrop)
        {
            controller = new BottomSheetModalController(window.Handler.MauiContext, page);
        }
        else
        {
            controller = new BottomSheetController(window.Handler.MauiContext, page);
        }
        page.Controller = controller;
        controller.Show();
    }

    internal static ViewGroup CreateLayout(BottomSheet page, IMauiContext mauiContext)
    {
        // The Android view for the page could already have a ContainerView as a parent if it was shown as a bottom sheet before
        if (((AView)page.Handler?.PlatformView)?.Parent is ContainerView cv) {
            cv.RemoveAllViews();
        }
        var containerView = page.ToContainerView(mauiContext);

        var r = page.Measure(page.Window.Width, page.Window.Height);

        containerView.LayoutParameters = new(ViewGroup.LayoutParams.MatchParent, (int)Math.Round(r.Request.Height * DeviceDisplay.MainDisplayInfo.Density));
        var layout = new FrameLayout(mauiContext.Context);
        if (page.HasHandle)
        {
            var handle = new BottomSheetDragHandleView(mauiContext.Context);
            layout.AddView(handle, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
        }
        layout.AddView(containerView);

        return layout;
    }

    internal static void LayoutDetents(BottomSheetBehavior behavior, ViewGroup container, BottomSheet page, double maxSheetHeight)
    {
        // Android supports the following detents:
        // - expanded (top of screen - offset)
        // - half expanded (using ratio of expanded - peekHeight)
        // - collapsed (using peekHeight)
        var detents = page.GetEnabledDetents().ToList();

        if (detents.Count == 0)
        {
            throw new Exception("Could not show BottomSheet: There me be at least one detent specified");
        }

        var heights = detents
            .Select(d =>
            {
                var val = d.GetHeight(page, maxSheetHeight);
                return val;
            })
            .OrderBy(d => -d)
            .ToList();


        if (heights.Count == 1)
        {
            behavior.FitToContents = true;
            behavior.SkipCollapsed = true;
            var top = heights[0];
            container.LayoutParameters.Height = (int)(top * DeviceDisplay.MainDisplayInfo.Density);
        }
        else if (heights.Count == 2)
        {
            behavior.FitToContents = true;
            behavior.SkipCollapsed = false;
            var top = heights[0];
            container.LayoutParameters.Height = (int)(top * DeviceDisplay.MainDisplayInfo.Density);
            var bottom = heights[1];

            behavior.PeekHeight = (int)(bottom * DeviceDisplay.MainDisplayInfo.Density);
        }
        else if (heights.Count == 3)
        {
            behavior.FitToContents = false;
            behavior.SkipCollapsed = false;
            var top = heights[0];
            var midway = heights[1];
            var bottom = heights[2];

            container.LayoutParameters.Height = (int)(top * DeviceDisplay.MainDisplayInfo.Density);

            // Set the top detent by offsetting the requested height from the maxHeight
            var topOffset = (maxSheetHeight - top) * DeviceDisplay.MainDisplayInfo.Density;
            behavior.ExpandedOffset = (int)topOffset;

            // Set the midway detent by calculating the ratio using the top detent info
            var ratio = midway / top;
            behavior.HalfExpandedRatio = (float)ratio;

            // Set the bottom detent using the peekHeight
            behavior.PeekHeight = (int)(bottom * DeviceDisplay.MainDisplayInfo.Density);
        }

        container.RequestLayout();
    }
    internal static Point GetLocationOnScreen(AView view)
    {
        int[] location = new int[2];
        view.GetLocationOnScreen(location);
        int x = location[0];
        int y = location[1];

        return new Point(x, y);
    }
}