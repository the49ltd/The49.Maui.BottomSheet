#nullable enable
using Microsoft.Maui.Handlers;

#if IOS
using PlatformView = UIKit.UIView;
#elif ANDROID
using PlatformView = Android.Views.View;
#else
using PlatformView = System.Object;
#endif


namespace The49.Maui.BottomSheet;

public partial class BottomSheetHandler : ContentViewHandler
{
    public static new IPropertyMapper<BottomSheet, BottomSheetHandler> Mapper =
        new PropertyMapper<BottomSheet, BottomSheetHandler>(ContentViewHandler.Mapper)
        {
            [nameof(IContentView.Background)] = MapBackground,
            [nameof(BottomSheet.HandleColor)] = MapHandleColor,
            [nameof(BottomSheet.HasBackdrop)] = MapHasBackdrop,
            [nameof(BottomSheet.SelectedDetent)] = MapSelectedDetent,
            [nameof(BottomSheet.CornerRadius)] = MapCornerRadius,
        };

    static void MapCornerRadius(BottomSheetHandler handler, BottomSheet sheet)
    {
        handler.PlatformUpdateCornerRadius(sheet);
    }

    static void MapHasBackdrop(BottomSheetHandler handler, BottomSheet sheet)
    {
        handler.PlatformUpdateHasBackdrop(sheet);
    }

    static void MapHandleColor(BottomSheetHandler handler, BottomSheet sheet)
    {
        handler.PlatformUpdateHandleColor(sheet);
    }

    public static new CommandMapper<BottomSheet, BottomSheetHandler> CommandMapper =
        new(ContentViewHandler.CommandMapper)
        {
            [nameof(BottomSheet.DismissAsync)] = MapDismiss,
        };

    static void MapDismiss(BottomSheetHandler handler, BottomSheet view, object? request)
    {
        handler.Dismiss(view, request ?? false);
    }

    public static void MapSelectedDetent(BottomSheetHandler handler, BottomSheet view)
    {
        handler.PlatformMapSelectedDetent(view);
    }

    internal void UpdateSelectedDetent(BottomSheet view)
    {
        PlatformUpdateSelectedDetent(view);
    }

    partial void PlatformMapSelectedDetent(BottomSheet view);
    partial void PlatformUpdateHandleColor(BottomSheet view);
    partial void PlatformUpdateHasBackdrop(BottomSheet view);
    partial void PlatformUpdateSelectedDetent(BottomSheet view);
    partial void PlatformUpdateCornerRadius(BottomSheet view);
    partial void Dismiss(BottomSheet view, object request);

    public BottomSheetHandler() : base(Mapper, CommandMapper)
    {
    }

    public BottomSheetHandler(IPropertyMapper? mapper)
        : base(mapper ?? Mapper, CommandMapper)
    {
    }

    public BottomSheetHandler(IPropertyMapper? mapper, CommandMapper? commandMapper)
        : base(mapper ?? Mapper, commandMapper ?? CommandMapper)
    {
    }

    new BottomSheet? VirtualView { get; }
    new PlatformView? PlatformView { get; }

}