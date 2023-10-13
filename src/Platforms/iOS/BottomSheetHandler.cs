namespace The49.Maui.BottomSheet;

public partial class BottomSheetHandler
{
    public static void MapBackground(BottomSheetHandler handler, BottomSheet view)
    {
        view.Controller.UpdateBackground();
    }
    partial void PlatformUpdateHandleColor(BottomSheet view)
    {
        // Not supported on iOS
    }

    partial void Dismiss(BottomSheet view, object request)
    {
        view.CachedDetents.Clear();
        view.Controller?.DismissViewController((bool)request, view.NotifyDismissed);
    }

    partial void PlatformMapSelectedDetent(BottomSheet view)
    {
        if (OperatingSystem.IsIOSVersionAtLeast(15))
        {
            view.Controller.UpdateSelectedIdentifierFromDetent();
        }
    }

    partial void PlatformUpdateSelectedDetent(BottomSheet view)
    {
        if (OperatingSystem.IsIOSVersionAtLeast(15))
        {
            view.Controller.UpdateSelectedDetent();
        }
    }
    partial void PlatformUpdateCornerRadius(BottomSheet view)
    {
        view.Controller.UpdateCornerRadius(view.CornerRadius);
    }
}