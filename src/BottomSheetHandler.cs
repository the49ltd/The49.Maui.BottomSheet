using Microsoft.Maui.Handlers;

namespace The49.Maui.BottomSheet;

public partial class BottomSheetHandler : ContentViewHandler
{
    public static new IPropertyMapper<BottomSheet, BottomSheetHandler> Mapper =
        new PropertyMapper<BottomSheet, BottomSheetHandler>(ContentViewHandler.Mapper)
        {
            [nameof(IContentView.Background)] = MapBackground,
        };


    public static new CommandMapper<BottomSheet, BottomSheetHandler> CommandMapper =
        new(ContentViewHandler.CommandMapper)
        {
            [nameof(BottomSheet.Dismiss)] = MapDismiss,
        };

    static void MapDismiss(BottomSheetHandler handler, BottomSheet view, object request)
    {
        handler.Dismiss(view, request);
    }

    partial void UpdateState(BottomSheet view);
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

}