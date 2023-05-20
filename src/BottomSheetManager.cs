namespace The49.Maui.BottomSheet;

internal partial class BottomSheetManager
{
    internal static void Show(Window window, BottomSheet sheet, bool animated)
    {
        PlatformShow(window, sheet, animated);
        sheet.SizeChanged += OnSizeChanged;
    }

    static void OnSizeChanged(object sender, EventArgs e)
    {
        PlatformLayout((BottomSheet)sender);
    }

    static partial void PlatformShow(Window window, BottomSheet sheet, bool animated);
    static partial void PlatformLayout(BottomSheet sheet);
}
