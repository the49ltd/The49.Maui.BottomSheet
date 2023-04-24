using UIKit;
namespace The49.Maui.BottomSheet;

internal partial class BottomSheetManager
{
    static partial void PlatformShow(Window window, BottomSheet page)
    {
        page.Parent = window;
        var controller = new BottomSheetPageViewController(window.Handler.MauiContext, page);
        page.Controller = controller;

#if IOS15_0_OR_GREATER
        controller.SheetPresentationController.PrefersGrabberVisible = page.HasHandle;
#endif

        var pageDetents = page.GetEnabledDetents();

#if IOS16_0_OR_GREATER
        var detents = pageDetents
            .Select((d, index) =>
            {
                return UISheetPresentationControllerDetent.Create($"detent{index}", (context) =>
                {
                    if (!page.CachedDetents.ContainsKey(index))
                    {
                        page.CachedDetents.Add(index, (float)d.GetHeight(page, context.MaximumDetentValue));
                    }
                    return page.CachedDetents[index];
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
        }
        else
        {
            detents = new UISheetPresentationControllerDetent[]
            {
                UISheetPresentationControllerDetent.CreateLargeDetent(),
                UISheetPresentationControllerDetent.CreateMediumDetent(),
            };
        }
#endif
#if IOS15_0_OR_GREATER
        controller.SheetPresentationController.Detents = detents;
#endif
#if IOS13_0_OR_GREATER
        controller.ModalInPresentation = !page.IsCancelable;
#endif
        var parent = Platform.GetCurrentUIViewController();

        parent.PresentViewController(controller, true, delegate { });
    }
}

