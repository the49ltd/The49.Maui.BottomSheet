namespace The49.Maui.BottomSheet;

public partial class BottomSheetHandler
{
    public static void MapBackground(BottomSheetHandler handler, BottomSheet page)
    {
        // Leave the background empty, the parent sheet handles the color
        page.Controller.UpdateBackground();
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
}