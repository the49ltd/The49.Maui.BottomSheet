using Microsoft.Maui.Platform;
using UIKit;

namespace The49.Maui.BottomSheet;

public class BottomSheetViewController : UIViewController
{
    IMauiContext _windowMauiContext;
    BottomSheet _sheet;

    public BottomSheetViewController(IMauiContext windowMauiContext, BottomSheet sheet) : base()
    {
        _windowMauiContext = windowMauiContext;
        _sheet = sheet;
        SheetPresentationController.Delegate = new BottomSheetControllerDelegate(_sheet);
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        var container = _sheet.ToPlatform(_windowMauiContext);

        var cv = new BottomSheetPageContainer(_sheet, container);

        View.AddSubview(cv);

        cv.TranslatesAutoresizingMaskIntoConstraints = false;

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            cv.TopAnchor.ConstraintEqualTo(View.TopAnchor),
            cv.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
            cv.BottomAnchor.ConstraintEqualTo(View.BottomAnchor),
            cv.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor)
        });

        if (_sheet.BackgroundBrush != null)
        {
            Paint paint = _sheet.BackgroundBrush;
            View.BackgroundColor = paint.ToColor().ToPlatform();
        }
        else
        {
            View.BackgroundColor = UIColor.SystemBackground;
        }
        _sheet.NotifyShowing();
    }
    public override void ViewDidLayoutSubviews()
    {
        base.ViewDidLayoutSubviews();
        SheetPresentationController.InvalidateDetents();
    }

    internal static UISheetPresentationControllerDetentIdentifier GetIdentifierForDetent(Detent d)
    {
        if (d is FullscreenDetent)
        {
            return UISheetPresentationControllerDetentIdentifier.Large;
        }
        else if (d is RatioDetent ratioDetent && ratioDetent.Ratio == .5)
        {
            return UISheetPresentationControllerDetentIdentifier.Medium;
        }
        return UISheetPresentationControllerDetentIdentifier.Unknown;
    }

    internal void UpdateSelectedIdentifierFromDetent()
    {
        if (_sheet.SelectedDetent is null)
        {
            return;
        }
        SheetPresentationController.AnimateChanges(() =>
        {
            SheetPresentationController.SelectedDetentIdentifier = GetIdentifierForDetent(_sheet.SelectedDetent);
        });
    }

    internal Detent GetSelectedDetent()
    {
        var detents = _sheet.GetEnabledDetents();
        return SheetPresentationController.SelectedDetentIdentifier switch
        {
            UISheetPresentationControllerDetentIdentifier.Medium => detents.FirstOrDefault(d => d is RatioDetent ratioDetent && ratioDetent.Ratio == .5f),
            UISheetPresentationControllerDetentIdentifier.Large => detents.FirstOrDefault(d => d is FullscreenDetent),
            UISheetPresentationControllerDetentIdentifier.Unknown or _ => null,
        };
    }

    internal void UpdateSelectedDetent()
    {
        _sheet.SelectedDetent = GetSelectedDetent();
    }
}

