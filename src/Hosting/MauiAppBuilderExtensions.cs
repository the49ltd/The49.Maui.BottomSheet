[assembly: XmlnsDefinition("https://schemas.the49.com/dotnet/2023/maui", "The49.Maui.BottomSheet")]
namespace The49.Maui.BottomSheet;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseBottomSheet(this MauiAppBuilder builder)
    {
        return builder.ConfigureMauiHandlers(cfg =>
        {
            cfg.AddHandler<BottomSheetPage, BottomSheetPageHandler>();
        });
    }
}
