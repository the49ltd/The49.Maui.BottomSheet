using Microsoft.Maui.Handlers;

namespace The49.Maui.BottomSheet;

public partial class BottomSheetPageHandler : PageHandler
{
    public static new IPropertyMapper<BottomSheetPage, BottomSheetPageHandler> Mapper =
        new PropertyMapper<BottomSheetPage, BottomSheetPageHandler>(PageHandler.Mapper)
        {
            [nameof(IContentView.Background)] = MapBackground,
            [nameof(Page.BackgroundColor)] = MapBackground,
        };


    public static new CommandMapper<BottomSheetPage, BottomSheetPageHandler> CommandMapper =
        new(ContentViewHandler.CommandMapper)
        {
            [nameof(BottomSheetPage.Dismiss)] = MapDismiss,
        };

    static void MapDismiss(BottomSheetPageHandler handler, BottomSheetPage view, object request)
    {
        handler.Dismiss(view, request);
    }

    partial void UpdateState(BottomSheetPage view);
    partial void Dismiss(BottomSheetPage view, object request);

    public BottomSheetPageHandler() : base(Mapper, CommandMapper)
    {
    }

    public BottomSheetPageHandler(IPropertyMapper? mapper)
        : base(mapper ?? Mapper, CommandMapper)
    {
    }

    public BottomSheetPageHandler(IPropertyMapper? mapper, CommandMapper? commandMapper)
        : base(mapper ?? Mapper, commandMapper ?? CommandMapper)
    {
    }

}