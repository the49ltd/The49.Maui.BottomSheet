using Maui.BindableProperty.Generator.Core;

namespace The49.Maui.BottomSheet;

[ContentProperty(nameof(Height))]
public partial class HeightDetent : Detent
{
    [AutoBindable]
    readonly double height;
    public override double GetHeight(BottomSheet page, double maxSheetHeight)
    {
        return Height;
    }
}
