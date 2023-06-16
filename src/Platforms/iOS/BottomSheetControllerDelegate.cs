using System.Runtime.Versioning;
using UIKit;

namespace The49.Maui.BottomSheet;

[SupportedOSPlatform("ios15.0")]
internal class BottomSheetControllerDelegate : UISheetPresentationControllerDelegate
{
    BottomSheet _sheet;

    public BottomSheetControllerDelegate(BottomSheet sheet) : base()
    {
        _sheet = sheet;
    }
    public override void DidDismiss(UIPresentationController presentationController)
    {
        _sheet.CachedDetents.Clear();
        _sheet.NotifyDismissed();
    }

    public override void DidChangeSelectedDetentIdentifier(UISheetPresentationController sheetPresentationController)
    {
        ((BottomSheetHandler)_sheet.Handler).UpdateSelectedDetent(_sheet);
    }
}

