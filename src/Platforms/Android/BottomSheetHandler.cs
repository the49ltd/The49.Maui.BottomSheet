namespace The49.Maui.BottomSheet;

public partial class BottomSheetHandler
{
    public static void MapBackground(BottomSheetHandler handler, BottomSheet sheet)
    {
        sheet.Controller.UpdateBackground();
    }
    partial void PlatformUpdateHandleColor(BottomSheet view)
    {
        view.Controller.UpdateHandleColor();
    }

    partial void Dismiss(BottomSheet view, object request)
    {
        view.Controller?.Dismiss((bool)request);
    }

    partial void PlatformUpdateSelectedDetent(BottomSheet view)
    {
        view.Controller.UpdateSelectedDetent();
    }

    partial void PlatformMapSelectedDetent(BottomSheet view)
    {
        view.Controller.UpdateStateFromDetent();
    }

    partial void PlatformUpdateHasBackdrop(BottomSheet view)
    {
        view.Controller.UpdateHasBackdrop();
    }

    partial void PlatformUpdateCornerRadius(BottomSheet view)
    {
        view.Controller.UpdateBackground();
    }
}