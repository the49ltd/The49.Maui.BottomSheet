namespace The49.Maui.BottomSheet;

internal partial class BottomSheetManager
{
    internal static void Show(Microsoft.Maui.Controls.Window window, BottomSheetPage page)
    {
        PlatformShow(window, page);
        page.LayoutChanged += Page_LayoutChanged;
    }

    static void Page_LayoutChanged(object sender, EventArgs e)
    {
        PlatformLayout((BottomSheetPage)sender);
    }

    static partial void PlatformShow(Microsoft.Maui.Controls.Window window, BottomSheetPage page);
    static partial void PlatformLayout(BottomSheetPage page);
}
