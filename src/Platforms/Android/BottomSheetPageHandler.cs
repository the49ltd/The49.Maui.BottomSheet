namespace The49.Maui.BottomSheet;

public partial class BottomSheetPageHandler
{
    public static void MapBackground(BottomSheetPageHandler handler, BottomSheetPage page)
    {
        // Leave the background empty, the parent sheet handles the color
    }

    partial void Dismiss(BottomSheetPage view, object request)
    {
        view.Controller?.Dismiss();
    }
}