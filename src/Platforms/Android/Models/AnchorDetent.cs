using AView = Android.Views.View;

namespace The49.Maui.BottomSheet;

public partial class AnchorDetent : Detent
{
    partial void UpdateHeight(BottomSheet page, double maxSheetHeight)
    {
        if (Anchor == null)
        {
            throw new Exception("Could not update Detent height: Anchor is not set");
        }
        var p = GetLocationOnScreen((AView)Anchor.Handler.PlatformView);
        var r = GetLocationOnScreen((AView)page.Handler.PlatformView);

        var offset = p - r;

        _height = offset.Height / DeviceDisplay.MainDisplayInfo.Density;
    }
    static Point GetLocationOnScreen(AView view)
    {
        int[] location = new int[2];
        view.GetLocationOnScreen(location);
        int x = location[0];
        int y = location[1];

        return new Point(x, y);
    }
}
