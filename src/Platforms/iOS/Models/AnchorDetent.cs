using System;
using UIKit;

namespace The49.Maui.BottomSheet;

public partial class AnchorDetent
{
    partial void UpdateHeight(BottomSheetPage page, double maxSheetHeight)
    {
        var r = page.Measure(page.Window.Width, maxSheetHeight);
        page.Arrange(new Rect(new Point(0, 0), r.Request));
        var pageView = (UIView)page.Handler.PlatformView;
        var targetView = (UIView)Anchor.Handler.PlatformView;

        pageView.SetNeedsLayout();
        pageView.LayoutIfNeeded();

        var targetOrigin = targetView.Superview.ConvertPointToView(targetView.Frame.Location, pageView);

        _height = targetOrigin.Y;
    }
}

