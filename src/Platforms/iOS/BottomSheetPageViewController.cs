using System;
using CoreGraphics;
using Microsoft.Maui.Platform;
using UIKit;

namespace The49.Maui.BottomSheet;

internal class BottomSheetPageContainer : UIView
{
    BottomSheetPage _page;
    UIView _view;

    internal BottomSheetPageContainer(BottomSheetPage page, UIView view)
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
public class BottomSheetPageViewController : UIViewController
{
    IMauiContext _windowMauiContext;
    BottomSheetPage _page;

    public BottomSheetPageViewController(IMauiContext windowMauiContext, BottomSheetPage page) : base()
    {
        _windowMauiContext = windowMauiContext;
        _page = page;
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        var container = _page.ToPlatform(_windowMauiContext);

        var cv = new BottomSheetPageContainer(_page, container);

        View = cv;
        if (_page.BackgroundBrush != null)
        {
            Paint paint = _page.BackgroundBrush;
            View.BackgroundColor = paint.ToColor().ToPlatform();
        }
        else
        {
            View.BackgroundColor = UIColor.SystemBackground;
        }
    }
}

