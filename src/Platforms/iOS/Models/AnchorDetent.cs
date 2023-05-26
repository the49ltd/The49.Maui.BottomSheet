using System;
using CoreGraphics;
using UIKit;

namespace The49.Maui.BottomSheet;

public partial class AnchorDetent
{
    partial void UpdateHeight(BottomSheet page, double maxSheetHeight)
    {
        var pageView = (UIView)page.Handler.PlatformView;
        pageView.Frame = new CGRect(new CGPoint(0, 0), new CGSize(page.Window.Width, maxSheetHeight));
        var targetView = (UIView)Anchor.Handler.PlatformView;

        pageView.SetNeedsLayout();
        pageView.LayoutIfNeeded();

        var targetOrigin = targetView.Superview.ConvertPointToView(targetView.Frame.Location, pageView);

        _height = targetOrigin.Y;
    }
}

