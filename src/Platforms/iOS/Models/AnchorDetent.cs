using UIKit;

namespace The49.Maui.BottomSheet;

public partial class AnchorDetent
{
    partial void UpdateHeight(BottomSheet page, double maxSheetHeight)
    {
        var pageView = (UIView)page.Handler.PlatformView;
        var targetView = (UIView)Anchor.Handler.PlatformView;

        var targetOrigin = targetView.Superview.ConvertPointToView(targetView.Frame.Location, pageView);

        _height = targetOrigin.Y;
    }
}

