namespace The49.Maui.BottomSheet;

public partial class BottomSheetHandler
{
    public static void MapBackground(BottomSheetHandler handler, BottomSheet page)
    {
        // Leave the background empty, the parent sheet handles the color
    }

    partial void Dismiss(BottomSheet view, object request)
    {
        view.Controller?.Dismiss();
    }
}