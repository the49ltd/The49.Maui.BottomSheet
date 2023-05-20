using CoreGraphics;
using UIKit;

namespace The49.Maui.BottomSheet;

internal class BottomSheetPageContainer : UIView
{
    BottomSheet _page;
    UIView _view;

    internal BottomSheetPageContainer(BottomSheet page, UIView view)
    {
        _page = page;
        _view = view;
        AddSubview(_view);
    }
    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        var r = _page.Measure(_page.Window.Width, _page.Window.Height);
        _view.Frame = new CGRect(0, 0, Bounds.Width, r.Request.Height);
    }
}

