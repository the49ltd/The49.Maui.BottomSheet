namespace The49.Maui.BottomSheet;

internal partial class BottomSheetManager
{
    static partial void PlatformShow(Window window, BottomSheet sheet, bool animated)
    {
        var controller = new BottomSheetController(window.Handler.MauiContext, sheet);
        sheet.Controller = controller;
        controller.Show(animated);
    }
}