using UIKit;
namespace The49.Maui.BottomSheet;

internal partial class BottomSheetManager
{
    static partial void PlatformShow(Window window, BottomSheet sheet, bool animated)
    {
        sheet.Parent = window;
        var controller = new BottomSheetViewController(window.Handler.MauiContext, sheet);
        sheet.Controller = controller;

        if (OperatingSystem.IsIOSVersionAtLeast(15))
        {
            controller.SheetPresentationController.PrefersGrabberVisible = sheet.HasHandle;
        }
        if (OperatingSystem.IsIOSVersionAtLeast(15))
        {
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
                    if (!OperatingSystem.IsIOSVersionAtLeast(16))
                    {
                        return null;
                    }
                    return UISheetPresentationControllerDetent.Create($"detent{index}", (context) =>
                    {
                        if (!sheet.CachedDetents.ContainsKey(index))
                        {
                            sheet.CachedDetents.Add(index, (float)d.GetHeight(sheet, context.MaximumDetentValue));
                        }
                        return sheet.CachedDetents[index];
                    });
                })
                .Where(d => d is not null)
                .ToList();

            if (detents.Count == 0)
            {
                largestDetentIdentifier = UISheetPresentationControllerDetentIdentifier.Medium;
                detents.Add(UISheetPresentationControllerDetent.CreateMediumDetent());
            }

            controller.SheetPresentationController.Detents = detents.ToArray();

            if (!sheet.HasBackdrop)
            {
                controller.SheetPresentationController.LargestUndimmedDetentIdentifier = largestDetentIdentifier;
            }

            controller.SheetPresentationController.SelectedDetentIdentifier = BottomSheetViewController.GetIdentifierForDetent(sheet.SelectedDetent);
        }

        if (OperatingSystem.IsIOSVersionAtLeast(13))
        {
            controller.ModalInPresentation = !sheet.IsCancelable;
        }

        var parent = Platform.GetCurrentUIViewController();

        parent.PresentViewController(controller, animated, sheet.NotifyShown);
    }
}

