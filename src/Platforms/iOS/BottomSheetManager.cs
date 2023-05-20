using UIKit;
namespace The49.Maui.BottomSheet;

internal partial class BottomSheetManager
{
    static partial void PlatformShow(Window window, BottomSheet sheet, bool animated)
    {
        sheet.Parent = window;
        var controller = new BottomSheetViewController(window.Handler.MauiContext, sheet);
        sheet.Controller = controller;

#if IOS15_0_OR_GREATER
        controller.SheetPresentationController.PrefersGrabberVisible = sheet.HasHandle;
#endif

        var largestDetentIdentifier = UISheetPresentationControllerDetentIdentifier.Unknown;

        if (!sheet.HasBackdrop)
        {
            controller.SheetPresentationController.LargestUndimmedDetentIdentifier = UISheetPresentationControllerDetentIdentifier.Large;
        }

        var pageDetents = sheet.GetEnabledDetents().ToList();

        if (pageDetents.Count == 0)
        {
            pageDetents = new List<Detent> { new ContentDetent() };
        }

#if IOS16_0_OR_GREATER
        var detents = pageDetents
            .Select((d, index) =>
            {
                if (d is FullscreenDetent)
                {
                    largestDetentIdentifier = UISheetPresentationControllerDetentIdentifier.Large;
                    return UISheetPresentationControllerDetent.CreateLargeDetent();
                }
                else if (d is RatioDetent ratioDetent && ratioDetent.Ratio == .5)
                {
                    largestDetentIdentifier = UISheetPresentationControllerDetentIdentifier.Medium;
                    return UISheetPresentationControllerDetent.CreateMediumDetent();
                }
                return UISheetPresentationControllerDetent.Create($"detent{index}", (context) =>
                {
                    
                    if (!sheet.CachedDetents.ContainsKey(index))
                    {
                        sheet.CachedDetents.Add(index, (float)d.GetHeight(sheet, context.MaximumDetentValue));
                    }
                    return sheet.CachedDetents[index];
                });
            }).ToArray();
#elif IOS15_0_OR_GREATER
        UISheetPresentationControllerDetent[] detents;
        if (pageDetents.Count == 1)
        {
            detents = new UISheetPresentationControllerDetent[]
            {
                UISheetPresentationControllerDetent.CreateMediumDetent(),
            };
            largestDetentIdentifier = UISheetPresentationControllerDetentIdentifier.Medium;
        }
        else
        {
            detents = new UISheetPresentationControllerDetent[]
            {
                UISheetPresentationControllerDetent.CreateLargeDetent(),
                UISheetPresentationControllerDetent.CreateMediumDetent(),
            };
            largestDetentIdentifier = UISheetPresentationControllerDetentIdentifier.Large;
        }
#endif
#if IOS15_0_OR_GREATER
        controller.SheetPresentationController.Detents = detents;
#endif
#if IOS13_0_OR_GREATER
        controller.ModalInPresentation = !sheet.IsCancelable;
#endif

        if (!sheet.HasBackdrop)
        {
            controller.SheetPresentationController.LargestUndimmedDetentIdentifier = largestDetentIdentifier;
        }

        controller.SheetPresentationController.SelectedDetentIdentifier = BottomSheetViewController.GetIdentifierForDetent(sheet.SelectedDetent);

        var parent = Platform.GetCurrentUIViewController();

        parent.PresentViewController(controller, animated, sheet.NotifyShown);
    }
}

