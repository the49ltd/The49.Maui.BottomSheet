namespace The49.Maui.BottomSheet;

public partial class BottomSheetPageHandler
{
    public static void MapBackground(BottomSheetPageHandler handler, BottomSheetPage page)
    {
        // Leave the background empty, the parent sheet handles the color
    }

    partial void Dismiss(BottomSheetPage view, object request)
    {
        view.Controller?.DismissViewController(true, delegate { });
    }

    partial void UpdateState(BottomSheetPage view)
    {
        // Can't do that in iOS16 as custom detents cannot be selected
    }
}