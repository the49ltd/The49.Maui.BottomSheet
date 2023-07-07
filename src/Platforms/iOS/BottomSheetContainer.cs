using CoreGraphics;
using UIKit;

namespace The49.Maui.BottomSheet;

internal class BottomSheetContainer : UIView
{
    BottomSheet _sheet;
    UIView _view;

    internal BottomSheetContainer(BottomSheet sheet, UIView view)
    {
        _sheet = sheet;
        _view = view;
        AddSubview(_view);
    }
    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        var r = _sheet.Measure(_sheet.Window.Width, _sheet.Window.Height);
        _view.Frame = new CGRect(0, 0, Bounds.Width, r.Request.Height);
        _sheet.Controller.Layout();
    }
}

