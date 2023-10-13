using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace The49.Maui.BottomSheet;

public class BottomSheetViewController : UIViewController
{
    IMauiContext _windowMauiContext;
    BottomSheet _sheet;
    NSObject? _keyboardDidHideObserver;

    public BottomSheetViewController(IMauiContext windowMauiContext, BottomSheet sheet) : base()
    {
        _windowMauiContext = windowMauiContext;
        _sheet = sheet;
        if (OperatingSystem.IsIOSVersionAtLeast(15))
        {
            SheetPresentationController.Delegate = new BottomSheetControllerDelegate(_sheet);
        }
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        var container = _sheet.ToPlatform(_windowMauiContext);

        var cv = new BottomSheetContainer(_sheet, container);

        View.AddSubview(cv);

        cv.TranslatesAutoresizingMaskIntoConstraints = false;

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            cv.TopAnchor.ConstraintEqualTo(View.TopAnchor),
            cv.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
            cv.BottomAnchor.ConstraintEqualTo(View.BottomAnchor),
            cv.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor)
        });

        UpdateBackground();
        _sheet.NotifyShowing();

        if (_keyboardDidHideObserver is null)
        {
            _keyboardDidHideObserver = UIKeyboard.Notifications.ObserveDidHide(KeyboardDidHide);
        }
    }

    void KeyboardDidHide(object sender, UIKeyboardEventArgs e)
    {
        Layout();
    }

    public void Layout()
    {
        _sheet.CachedDetents.Clear();
        if (OperatingSystem.IsIOSVersionAtLeast(16))
        {
            SheetPresentationController.InvalidateDetents();
        }
    }
    internal void UpdateBackground()
    {
        if (_sheet?.BackgroundBrush != null)
        {
            Paint paint = _sheet.BackgroundBrush;
            View.BackgroundColor = paint.ToColor().ToPlatform();
        }
        else
        {
            if (OperatingSystem.IsIOSVersionAtLeast(13))
            {
                View.BackgroundColor = UIColor.SystemBackground;
            }
        }
    }
    public override void ViewDidLayoutSubviews()
    {
        base.ViewDidLayoutSubviews();
        Layout();
    }

    [SupportedOSPlatform("ios15.0")]
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

    [SupportedOSPlatform("ios15.0")]
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

    [SupportedOSPlatform("ios15.0")]
    internal Detent GetSelectedDetent()
    {
        if (!OperatingSystem.IsIOSVersionAtLeast(15))
        {
            return null;
        }
        var detents = _sheet.GetEnabledDetents();
        return SheetPresentationController.SelectedDetentIdentifier switch
        {
            UISheetPresentationControllerDetentIdentifier.Medium => detents.FirstOrDefault(d => d is RatioDetent ratioDetent && ratioDetent.Ratio == .5f),
            UISheetPresentationControllerDetentIdentifier.Large => detents.FirstOrDefault(d => d is FullscreenDetent),
            UISheetPresentationControllerDetentIdentifier.Unknown or _ => null,
        };
    }

    [SupportedOSPlatform("ios15.0")]
    internal void UpdateSelectedDetent()
    {
        _sheet.SelectedDetent = GetSelectedDetent();
    }

    internal void UpdateCornerRadius(double cornerRadius)
    {
        if (!OperatingSystem.IsIOSVersionAtLeast(15))
        {
            return;
        }
        SheetPresentationController.PreferredCornerRadius = (NFloat)cornerRadius;
    }
}

